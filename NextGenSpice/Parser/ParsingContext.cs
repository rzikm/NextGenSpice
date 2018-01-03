﻿using System.Collections.Generic;
using System.Linq;
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
            DeferredStatements = new List<DeferredStatement>();
            SimulationStatements = new List<SimulationStatement>();
            PrintStatements = new List<PrintStatement>();
            CircuitBuilder = new CircuitBuilder();
        }

        public ISet<string> KnownAnalysisTypes { get;  }
        public SymbolTable SymbolTable { get;  }
        public List<ErrorInfo> Errors { get;  }
        public List<DeferredStatement> DeferredStatements { get; }
        public List<SimulationStatement> SimulationStatements { get;  }
        public List<PrintStatement> PrintStatements { get; }
        public CircuitBuilder CircuitBuilder { get; }
    }
}