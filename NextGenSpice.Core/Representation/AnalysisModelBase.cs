using System.Collections.Generic;
using NextGenSpice.Elements;

namespace NextGenSpice.Representation
{
    public abstract class AnalysisModelBase<TElement>
    {
        public IReadOnlyList<TElement> Elements { get; set; }
    }

    public class LargeSignalCircuitModel : AnalysisModelBase<ILargeSignalDeviceModel>
    {
        
    }
}