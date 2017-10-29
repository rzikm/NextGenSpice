using System.Collections.Generic;

namespace NextGenSpice.Core.Representation
{
    public interface IAnalysisModel<TElement>
    {
        IReadOnlyList<TElement> Elements { get; }
    }
}