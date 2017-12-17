using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NextGenSpice
{
    public class SymbolTable
    {
        public SymbolTable()
        {
            DefinedSymbols = new HashSet<string>();
            Models = new Dictionary<string, ModelStatement>();
            DefinedElements = new HashSet<string>();
            NodeIndices = new Dictionary<string, int> {["0"] = 0}; // enforce ground node on index 0
        }

        public ISet<string> DefinedSymbols { get; }
        public ISet<string> DefinedElements { get; }
        public IDictionary<string, ModelStatement> Models { get; }
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
    }
}