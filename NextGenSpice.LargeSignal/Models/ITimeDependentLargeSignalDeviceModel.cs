using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public interface ITimeDependentLargeSignalDeviceModel : ILargeSignalDeviceModel
    {
        void UpdateTimeDependentModel(ISimulationContext context);
        void RollbackTimeDependentModel();

        void ApplyTimeDependentModelValues(IEquationSystem equation, ISimulationContext context);
    }
}