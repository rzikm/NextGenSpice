using NextGenSpice.Equations;
using NextGenSpice.Representation;

namespace NextGenSpice.Elements
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