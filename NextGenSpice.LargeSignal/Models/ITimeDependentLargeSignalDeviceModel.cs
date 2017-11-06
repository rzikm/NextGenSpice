using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public interface ITimeDependentLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void AdvanceTimeDependentModel(SimulationContext context);
        void RollbackTimeDependentModel();

        void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context);
    }
}