using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.LargeSignal.Models
{
    public interface ILargeSignalDeviceModel : IAnalysisDeviceModel<LargeSignalCircuitModel>
    {
        string Name { get; }

        void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context);

        void ApplyModelValues(IEquationEditor equations, ISimulationContext context);

        void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context);
        
        void OnDcBiasEstablished(ISimulationContext context);

        bool IsNonlinear { get; }
        bool IsTimeDependent { get; }
    }
}