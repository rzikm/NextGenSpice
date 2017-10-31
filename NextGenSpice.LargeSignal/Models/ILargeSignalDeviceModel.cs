using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.LargeSignal.Models
{
    public interface ILargeSignalDeviceModel : IAnalysisDeviceModel<LargeSignalCircuitModel>
    {
        void Initialize(IEquationSystemBuilder builder);
    }

    public interface ILinearLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void ApplyLinearModelValues(IEquationEditor equationSystem, SimulationContext context);
    }
}