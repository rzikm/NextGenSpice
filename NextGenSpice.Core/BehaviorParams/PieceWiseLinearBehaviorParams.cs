using System.Collections.Generic;

namespace NextGenSpice.Core.BehaviorParams
{
    public class PieceWiseLinearBehaviorParams : SourceBehaviorParams
    {
        public IReadOnlyDictionary<double, double> DefinitionPoints { get; set; }
        public double InitialValue { get; set; }
        public double? RepeatStart { get; set; }
    }
}