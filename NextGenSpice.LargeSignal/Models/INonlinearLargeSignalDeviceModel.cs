using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public interface INonlinearLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void UpdateNonlinearModel(ISimulationContext context);

        void ApplyNonlinearModelValues(IEquationSystem equation, ISimulationContext context);
    }
}