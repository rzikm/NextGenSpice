using System.Collections.Generic;

namespace NextGenSpice.Core.Representation
{
    public interface IAnalysisCircuitModel<TElement>
    {
        IReadOnlyList<TElement> Elements { get; }
    }
}