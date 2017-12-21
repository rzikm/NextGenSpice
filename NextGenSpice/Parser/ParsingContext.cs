﻿using System.Collections.Generic;
using NextGenSpice.Core.Circuit;

namespace NextGenSpice
{
    public class ParsingContext
    {
        public ParsingContext()
        {
            KnownAnalysisTypes = new HashSet<string>();
            SymbolTable = new SymbolTable();
            Errors = new List<ErrorInfo>();
            ElementStatements = new List<ElementStatement>();
            ModelStatements = new List<ModelStatement>();
            SimulationStatements = new List<TranSimulationStatement>();
            PrintStatements = new List<PrintStatement>();
            CircuitBuilder = new CircuitBuilder();
        }

        public ISet<string> KnownAnalysisTypes { get;  }
        public SymbolTable SymbolTable { get;  }
        public List<ErrorInfo> Errors { get;  }
        public List<ElementStatement> ElementStatements { get; }
        public List<ModelStatement> ModelStatements { get;  }
        public List<TranSimulationStatement> SimulationStatements { get;  }
        public List<PrintStatement> PrintStatements { get; }
        public CircuitBuilder CircuitBuilder { get; }
    }
}