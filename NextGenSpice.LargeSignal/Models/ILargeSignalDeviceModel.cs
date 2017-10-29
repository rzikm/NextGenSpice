using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.LargeSignal.Models
{
    public interface ILargeSignalDeviceModel : IAnalysisDeviceModel<LargeSignalCircuitModel>
    {
        void Initialize();
    }

    public interface ILinearLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context);
    }
}