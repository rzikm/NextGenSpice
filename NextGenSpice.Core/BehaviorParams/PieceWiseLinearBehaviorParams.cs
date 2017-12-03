using System.Collections.Generic;

namespace NextGenSpice.Core.BehaviorParams
{
    public class PieceWiseLinearBehaviorParams : SourceBehaviorParams
    {
        public IReadOnlyDictionary<double, double> DefinitionPoints { get; set; }
        public bool Repeat { get; set; }
    }
}