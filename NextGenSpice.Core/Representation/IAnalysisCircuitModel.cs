using System.Collections.Generic;

namespace NextGenSpice.Core.Representation
{
    public interface IAnalysisCircuitModel<out TElement>
    {
        IReadOnlyList<TElement> Elements { get; }
    }
}