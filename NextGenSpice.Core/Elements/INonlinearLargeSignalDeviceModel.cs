using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public interface INonlinearLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void UpdateNonlinearModel(SimulationContext context);

        void ApplyNonlinearModelValues(IEquationSystem equationSystem, SimulationContext context);
    }
}