using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public interface ILinearLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void ApplyLinearModelValues(IEquationEditor equation, SimulationContext context);
    }
}