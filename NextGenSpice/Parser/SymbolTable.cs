using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace NextGenSpice
{
    public class SymbolTable
    {
        public SymbolTable()
        {
            DefinedSymbols = new HashSet<string>();
            DefinedElements = new HashSet<string>();

            Models = ((ModelType[]) Enum.GetValues(typeof(ModelType))).ToDictionary(type => type,
                type => new Dictionary<string, object>());

            NodeIndices = new Dictionary<string, int> {["0"] = 0}; // enforce ground node on index 0
        }

        public ISet<string> DefinedSymbols { get; }
        public ISet<string> DefinedElements { get; }
        public Dictionary<ModelType, Dictionary<string, object>> Models { get; }

        public IDictionary<string, int> NodeIndices { get; }

        public bool DefineElement(string name)
        {
            if (DefinedSymbols.Contains(name))
                return false;
            DefinedSymbols.Add(name);
            return DefinedElements.Add(name);
        }

        public bool TryGetNodeIndex(string name, out int index)
        {
            index = 0;
            if (string.IsNullOrWhiteSpace(name)) throw new InvalidEnumArgumentException($"Parameter {nameof(name)} must be nonempty.");
            if (NodeIndices.TryGetValue(name, out index)) return true;

            if (DefinedSymbols.Contains(name)) return false; // symbol 'name' is already used for something else

            NodeIndices[name] = index = NodeIndices.Count;
            DefinedSymbols.Add(name);

            return true;
        }

        public IEnumerable<string> GetNodeNames(IEnumerable<int> exNodes)
        {
            return exNodes.Select(id => NodeIndices.First(kvp => kvp.Value == id).Key);
        }
    }
}