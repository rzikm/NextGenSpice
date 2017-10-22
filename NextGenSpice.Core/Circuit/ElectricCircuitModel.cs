using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Elements;

namespace NextGenSpice.Circuit
{
    public class ElectricCircuitModel : ICircuitModel
    {
        public ElectricCircuitModel(CircuitNode[] nodes, IEnumerable<ILargeSignalDeviceModel> elements)
        {
            this.Nodes = nodes;
            this.Elements = elements.ToArray();
            LinearElements = Elements.OfType<ILinearLargeSignalDeviceModel>().ToArray();
            NonlinearElements = Elements.OfType<INonlinearLargeSignalDeviceModel>().ToArray();
            TimeDependentElements = Elements.OfType<ITimeDependentLargeSignalDeviceModel>().ToArray();
        }
        public CircuitNode[] Nodes { get; }
        public ILargeSignalDeviceModel[] Elements { get; }

        public ILinearLargeSignalDeviceModel[] LinearElements { get; }
        public INonlinearLargeSignalDeviceModel[] NonlinearElements { get; }
        public ITimeDependentLargeSignalDeviceModel[] TimeDependentElements { get; }
        public bool IsLinear => NonlinearElements.Length == 0;

    }
}