using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public interface ITimeDependentLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void UpdateTimeDependentModel(SimulationContext context);

        void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context);
    }
}