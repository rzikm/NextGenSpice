using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal
{
    public class LargeSignalCircuitModel : IAnalysisModel<ILargeSignalDeviceModel>
    {
        public LargeSignalCircuitModel(IEnumerable<double> initialVoltages, List<ILargeSignalDeviceModel> elements)
        {
            NodeVoltages = initialVoltages.ToArray();
            Elements = elements;
            LinearElements = elements.OfType<ILinearLargeSignalDeviceModel>().ToList();
            NonlinearElements = elements.OfType<INonlinearLargeSignalDeviceModel>().ToList();
            TimeDependentElements = elements.OfType<ITimeDependentLargeSignalDeviceModel>().ToList();
        }

        public double[] NodeVoltages { get; }
        public int NodeCount => NodeVoltages.Length;
        public IReadOnlyList<ILinearLargeSignalDeviceModel> LinearElements { get; }
        public IReadOnlyList<INonlinearLargeSignalDeviceModel> NonlinearElements { get; }
        public IReadOnlyList<ITimeDependentLargeSignalDeviceModel> TimeDependentElements { get; }
        public bool IsLinear => NonlinearElements.Count == 0;
        public IReadOnlyList<ILargeSignalDeviceModel> Elements { get; set; }
    }
}