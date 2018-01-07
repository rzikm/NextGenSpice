using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using NextGenSpice.Parser.Statements.Models;

namespace NextGenSpice.Parser
{
    /// <summary>
    /// Class aggregating all encountered symbols in the input file.
    /// </summary>
    public class SymbolTable
    {
        public SymbolTable()
        {
            DefinedElements = new HashSet<string>();

            Models = ((DeviceType[]) Enum.GetValues(typeof(DeviceType))).ToDictionary(type => type,
                type => new Dictionary<string, object>());

            NodeIndices = new Dictionary<string, int> {["0"] = 0}; // enforce ground node on index 0
        }

        /// <summary>
        /// Set of all device element identifiers.
        /// </summary>
        public ISet<string> DefinedElements { get; }

        /// <summary>
        /// Sets of all device models (parameter sets) for each device type.
        /// </summary>
        public Dictionary<DeviceType, Dictionary<string, object>> Models { get; }

        /// <summary>
        /// Set of all node identifiers with associated ids that will be used during simulation.
        /// </summary>
        public IDictionary<string, int> NodeIndices { get; }

        /// <summary>
        /// Returns whether given symbol is already used for a device or node.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //TODO: consider including models => nodes/elements/models(even for different devices) shall have unique names
        private bool IsDefined(string symbol)
        {
            return DefinedElements.Contains(symbol) || NodeIndices.ContainsKey(symbol);
        }

        /// <summary>
        /// Adds given symbol to the set of element names, returns true if it not already used.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool TryDefineElement(string name)
        {
            return !IsDefined(name) && DefinedElements.Add(name);
        }

        /// <summary>
        /// Tries to get node index corresponding to given node name. Returns true on success.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool TryGetNodeIndex(string name, out int index)
        {
            index = 0;
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidEnumArgumentException($"Parameter {nameof(name)} must be nonempty.");
            if (NodeIndices.TryGetValue(name, out index)) return true;

            if (IsDefined(name)) return false; // symbol 'name' is already used for a device

            NodeIndices[name] = index = NodeIndices.Count;

            return true;
        }

        /// <summary>
        /// Gives set of node names for given set of indexes
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public IEnumerable<string> GetNodeNames(IEnumerable<int> indexes)
        {
            return indexes.Select(id => NodeIndices.First(kvp => kvp.Value == id).Key);
        }
    }
}