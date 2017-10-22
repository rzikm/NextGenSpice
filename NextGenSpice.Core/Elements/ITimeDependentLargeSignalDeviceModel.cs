using NextGenSpice.Equations;

namespace NextGenSpice.Elements
{
    public interface ITimeDependentLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void UpdateTimeDependentModel(SimulationContext context);

        void ApplyTimeDependentModelValues(IEquationSystem equationSystem, SimulationContext context);
    }
}