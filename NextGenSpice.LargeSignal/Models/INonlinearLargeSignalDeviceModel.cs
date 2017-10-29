using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public interface INonlinearLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void UpdateNonlinearModel(SimulationContext context);

        void ApplyNonlinearModelValues(IEquationSystem equationSystem, SimulationContext context);
    }
}