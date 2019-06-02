using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Parser
{
	/// <summary>Class aggregating all encountered symbols in the input file.</summary>
	public class SymbolTable : ISymbolTable
	{
		private readonly SymbolTable parent;

		public SymbolTable(SymbolTable parent = null)
		{
			this.parent = parent;

			DefinedDevices = new HashSet<string>();
			Models = new Dictionary<Type, IDictionary<string, object>>();
			NodeIndices = new Dictionary<string, int> {["0"] = 0}; // enforce ground node on index 0
			SubcircuitDevices = new Dictionary<string, ISubcircuitDefinition>();
		}

		private IDictionary<Type, IDictionary<string, object>> Models { get; }

		/// <summary>Set of all device device identifiers from current scope.</summary>
		private ISet<string> DefinedDevices { get; }

		/// <summary>Set of all node identifiers from current scope with associated ids that will be used during simulation.</summary>
		private IDictionary<string, int> NodeIndices { get; }

		/// <summary>Set of all subcircuits defined in current scope.</summary>
		private IDictionary<string, ISubcircuitDefinition> SubcircuitDevices { get; }


		/// <summary>Gets the model parameters of given type associated with given name.</summary>
		/// <param name="modelType">Type of the model parameters.</param>
		/// <param name="name">Name of the model.</param>
		/// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
		/// <returns>True if given model was found, false otherwise.</returns>
		public bool TryGetModel(Type modelType, string name, out object model)
		{
			var t = this;

			while (t != null && (!t.Models.ContainsKey(modelType) || !t.Models[modelType].ContainsKey(name))) t = t.parent;

			model = null;
			if (t == null) return false;

			return t.Models[modelType].TryGetValue(name, out model);
		}

		/// <summary>Gets the model parameters of given type associated with given name.</summary>
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

		/// <summary>Gets model of given type associated with given name.</summary>
		/// <param name="modelType">Type of the model parameters.</param>
		/// <param name="name">Name of the model.</param>
		/// <returns>The model.</returns>
		public object GetModel(Type modelType, string name)
		{
			if (!TryGetModel(modelType, name, out var model))
				throw new ArgumentException($"There is no model of type {modelType}, named {name}.");
			return model;
		}

		/// <summary>Gets model of given type associated with given name.</summary>
		/// <typeparam name="T">Type of the model parameters.</typeparam>
		/// <param name="name">Name of the model.</param>
		/// <returns>The model.</returns>
		public T GetModel<T>(string name)
		{
			return (T) GetModel(typeof(T), name);
		}

		/// <summary>Adds model of given type and name to the symbol tables.</summary>
		/// <param name="modelType">Type of the model parameters.</param>
		/// <param name="name">Name of the model.</param>
		/// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
		public void AddModel(Type modelType, object model, string name)
		{
			if (!Models.ContainsKey(modelType))
				Models[modelType] = new Dictionary<string, object>();

			Models[modelType].Add(name, model);
		}

		/// <summary>Adds model of given type and name to the symbol tables.</summary>
		/// <typeparam name="T">Type of the model type.</typeparam>
		/// <param name="name">Name of the model.</param>
		/// <param name="model">If this function returns true, contains the found model, otherwise null.</param>
		public void AddModel<T>(T model, string name)
		{
			AddModel(typeof(T), model, name);
		}

		/// <summary>
		///   Returns collection of all models from the symbol table.
		/// </summary>
		/// <returns></returns>
		public IReadOnlyDictionary<Type, IReadOnlyDictionary<string, object>> GetAllModels()
		{
			return Models.ToDictionary(i => i.Key, i => i.Value as IReadOnlyDictionary<string, object>);
		}

		/// <summary>Adds subcircuit under given name to the symbol tables.</summary>
		/// <param name="name">Name of the subcircuit.</param>
		/// <param name="subcircuit">The subcircuit deinition.</param>
		public void AddSubcircuit(string name, ISubcircuitDefinition subcircuit)
		{
			SubcircuitDevices.Add(name, subcircuit);
		}

		/// <summary>Gets the subcircuit associated with given name.</summary>
		/// <param name="name">Name of the subcircuit.</param>
		/// <param name="subcircuit">Out variable to store found model in.</param>
		/// <returns></returns>
		public bool TryGetSubcircuit(string name, out ISubcircuitDefinition subcircuit)
		{
			var t = this;

			while (!t?.SubcircuitDevices.ContainsKey(name) ?? false) t = t.parent;

			subcircuit = null;
			if (t == null) return false;

			return t.SubcircuitDevices.TryGetValue(name, out subcircuit);
		}

		/// <summary>Returns the subcircuit instance with corresponding name.</summary>
		/// <param name="name">The subcircuit name</param>
		/// <returns></returns>
		public ISubcircuitDefinition GetSubcircuit(string name)
		{
			if (!TryGetSubcircuit(name, out var result))
				throw new ArgumentException($"Subcircuit with name '{name}' does not exist");
			return result;
		}

		/// <summary>Returns all subcircuits from the local symbol table.</summary>
		/// <returns></returns>
		public IEnumerable<ISubcircuitDefinition> GetLocalSubcircuits()
		{
			return SubcircuitDevices.Values;
		}

		/// <summary>Adds given symbol to the set of device names, returns true if it not already used.</summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool TryDefineDevice(string name)
		{
			return !IsDefined(name) && DefinedDevices.Add(name);
		}

		/// <summary>Tries to get node index corresponding to given node name. Returns true on success.</summary>
		/// <param name="name">Name of the node.</param>
		/// <param name="index">Index of the node.</param>
		/// <returns></returns>
		public bool TryGetNodeIndex(string name, out int index)
		{
			index = 0;
			if (string.IsNullOrWhiteSpace(name))
				throw new InvalidEnumArgumentException($"Parameter {nameof(name)} must be nonempty.");

			return NodeIndices.TryGetValue(name, out index);
		}

		/// <summary>Defines new node with given name and assigns it a new index. Returns true on success.</summary>
		/// <param name="name">Name of the node.</param>
		/// <param name="index">Index of the node.</param>
		/// <returns></returns>
		public bool TryGetOrCreateNode(string name, out int index)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new InvalidEnumArgumentException($"Parameter {nameof(name)} must be nonempty.");

			if (TryGetNodeIndex(name, out index)) return true;
			if (IsDefined(name)) return false;

			NodeIndices[name] = index = NodeIndices.Count;
			return true;
		}

		/// <summary>Gives set of node names for given set of indexes</summary>
		/// <param name="indexes"></param>
		/// <returns></returns>
		public IEnumerable<string> GetNodeNames(IEnumerable<int> indexes)
		{
			return indexes.Select(id => NodeIndices.First(kvp => kvp.Value == id).Key);
		}

		/// <summary>Returns dictionary with mappings from node id to their respective names.</summary>
		/// <returns></returns>
		public IDictionary<int, string> GetNodeIdMappings()
		{
			return NodeIndices.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
		}

		/// <summary>Returns whether given symbol is already used for a device or node.</summary>
		/// <param name="symbol"></param>
		/// <returns></returns>
		private bool IsDefined(string symbol)
		{
			return DefinedDevices.Contains(symbol) || NodeIndices.ContainsKey(symbol);
		}

		//        /// <summary>Enters a new scope for managing entries inside subcircuit.</summary>
		//        public void EnterSubcircuit()
		//        {
		//            if (defaultsScope.Models == null)
		//                throw new InvalidOperationException("Symbol table defaults must be frozen first");
		//            scopes.Push(DuplicateStackEntry(defaultsScope));
		//        }
		//
		//        /// <summary>Exits current subcircuit scope and returns to upper scope.</summary>
		//        public void ExitSubcircuit()
		//        {
		//            scopes.Pop();
		//        }
		//
		//        private struct StackEntry
		//        {
		//            public StackEntry(ISet<string> definedDevices, IDictionary<string, int> nodeIndices,
		//                IDictionary<Type, IDictionary<string, object>> models, IDictionary<string, ISubcircuitDefinition> subcircuits)
		//            {
		//                DefinedDevices = definedDevices;
		//                NodeIndices = nodeIndices;
		//                Models = models;
		//                Subcircuits = subcircuits;
		//            }
		//
		//            public ISet<string> DefinedDevices { get; }
		//            public IDictionary<string, int> NodeIndices { get; }
		//            public IDictionary<Type, IDictionary<string, object>> Models { get; }
		//            public IDictionary<string, ISubcircuitDefinition> Subcircuits { get; }
		//        }
	}
}