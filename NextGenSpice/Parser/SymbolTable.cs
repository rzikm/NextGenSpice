using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NextGenSpice.Parser
{
    /// <summary>
    ///     Class aggregating all encountered symbols in the input file.
    /// </summary>
    public class SymbolTable
    {
        private readonly Stack<StackEntry> scopes;

        private Dictionary<Type, Dictionary<string, object>> models;


        public SymbolTable()
        {
            DefinedElements = new HashSet<string>();

            models = new Dictionary<Type, Dictionary<string, object>>();
            scopes = new Stack<StackEntry>();

            NodeIndices = new Dictionary<string, int> {["0"] = 0}; // enforce ground node on index 0
        }

        /// <summary>
        ///     How deep into nested subcircuit are we during parsing. 0 means that we are in the global scope.
        /// </summary>
        public int SubcircuitDepth => scopes.Count;

        /// <summary>
        ///     Set of all device element identifiers.
        /// </summary>
        public ISet<string> DefinedElements { get; private set; }

        /// <summary>
        ///     Set of all node identifiers with associated ids that will be used during simulation.
        /// </summary>
        public IDictionary<string, int> NodeIndices { get; private set; }

        /// <summary>
        ///     Gets the model parameters of given type associated with given name.
        /// </summary>
        /// <param name="modelType">Type of the model parameters.</param>
        /// <param name="name">Name of the model.</param>
        /// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
        /// <returns>True if given model was found, false otherwise.</returns>
        public bool TryGetModel(Type modelType, string name, out object model)
        {
            // search current scope;
            var succ = TryGetModel(modelType, name, out model, models);

            if (!succ) // if not found, search upper scopes
                foreach (var frame in scopes)
                {
                    succ = TryGetModel(modelType, name, out model, frame.Models);
                    if (succ) break; // model found
                }

            return succ;
        }

        private bool TryGetModel(Type modelType, string name, out object model,
            Dictionary<Type, Dictionary<string, object>> modelSet)
        {
            model = null;
            if (!modelSet.ContainsKey(modelType)) return false;
            return modelSet[modelType].TryGetValue(name, out model);
        }

        /// <summary>
        ///     Gets the model parameters of given type associated with given name.
        /// </summary>
        /// <typeparam name="T">Type of the model parameters.</typeparam>
        /// <param name="name">Name of the model.</param>
        /// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
        /// <returns>True if given model was found, false otherwise.</returns>
        public bool TryGetModel<T>(string name, out T model)
        {
            var ret = TryGetModel(typeof(T), name, out var m);
            model = (T) m;
            return ret;
        }

        /// <summary>
        ///     Gets model of given type associated with given name.
        /// </summary>
        /// <param name="modelType">Type of the model parameters.</param>
        /// <param name="name">Name of the model.</param>
        /// <returns>The model.</returns>
        public object GetModel(Type modelType, string name)
        {
            if (!TryGetModel(modelType, name, out var model))
                throw new ArgumentException($"There is no model of type {modelType}, named {name}.");
            return model;
        }

        /// <summary>
        ///     Gets model of given type associated with given name.
        /// </summary>
        /// <typeparam name="T">Type of the model parameters.</typeparam>
        /// <param name="name">Name of the model.</param>
        /// <returns>The model.</returns>
        public T GetModel<T>(string name)
        {
            return (T) GetModel(typeof(T), name);
        }

        /// <summary>
        ///     Adds model of given type and name to the symbol tables.
        /// </summary>
        /// <param name="modelType">Type of the model parameters.</param>
        /// <param name="name">Name of the model.</param>
        /// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
        public void AddModel(Type modelType, object model, string name)
        {
            if (!models.ContainsKey(modelType))
                models[modelType] = new Dictionary<string, object>();

            models[modelType].Add(name, model);
        }

        /// <summary>
        ///     Adds model of given type and name to the symbol tables.
        /// </summary>
        /// <typeparam name="T">Type of the model type.</typeparam>
        /// <param name="name">Name of the model.</param>
        /// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
        public void AddModel<T>(T model, string name)
        {
            AddModel(typeof(T), model, name);
        }

        /// <summary>
        ///     Returns whether given symbol is already used for a device or node.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool IsDefined(string symbol)
        {
            return DefinedElements.Contains(symbol) || NodeIndices.ContainsKey(symbol);
        }

        /// <summary>
        ///     Adds given symbol to the set of element names, returns true if it not already used.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool TryDefineElement(string name)
        {
            return !IsDefined(name) && DefinedElements.Add(name);
        }

        /// <summary>
        ///     Tries to get node index corresponding to given node name. Returns true on success.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool TryGetNodeIndex(string name, out int index)
        {
            index = 0;
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidEnumArgumentException($"Parameter {nameof(name)} must be nonempty.");
            if (NodeIndices.TryGetValue(name, out index)) return true;

            if (IsDefined(name)) return false; // symbol 'name' is already used for a device

            NodeIndices[name] = index = NodeIndices.Count;

            return true;
        }

        /// <summary>
        ///     Gives set of node names for given set of indexes
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public IEnumerable<string> GetNodeNames(IEnumerable<int> indexes)
        {
            return indexes.Select(id => NodeIndices.First(kvp => kvp.Value == id).Key);
        }

        /// <summary>
        ///     Enters a new scope for managing entries inside subcircuit.
        /// </summary>
        public void EnterSubcircuit()
        {
            scopes.Push(new StackEntry(DefinedElements, NodeIndices, models));

            DefinedElements = new HashSet<string>();
            NodeIndices = new Dictionary<string, int> {["0"] = 0};
            models = new Dictionary<Type, Dictionary<string, object>>();
        }

        /// <summary>
        ///     Exits current subcircuit scope and returns to upper scope.
        /// </summary>
        public void ExitSubcircuit()
        {
            var frame = scopes.Pop();
            (DefinedElements, NodeIndices, models) = frame;
        }

        private struct StackEntry
        {
            public StackEntry(ISet<string> definedElements, IDictionary<string, int> nodeIndices,
                Dictionary<Type, Dictionary<string, object>> models)
            {
                DefinedElements = definedElements;
                NodeIndices = nodeIndices;
                Models = models;
            }

            public ISet<string> DefinedElements { get; }
            public IDictionary<string, int> NodeIndices { get; }
            public Dictionary<Type, Dictionary<string, object>> Models { get; }

            public void Deconstruct(out ISet<string> definedElements, out IDictionary<string, int> nodeIndices,
                out Dictionary<Type, Dictionary<string, object>> models)
            {
                definedElements = DefinedElements;
                nodeIndices = NodeIndices;
                models = Models;
            }
        }
    }
}