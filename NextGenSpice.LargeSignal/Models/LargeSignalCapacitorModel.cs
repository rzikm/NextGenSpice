using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalCapacitorModel : TwoNodeLargeSignalModel<CapacitorElement>, ITimeDependentLargeSignalDeviceModel
    {
        private double Vc;

        public LargeSignalCapacitorModel(CapacitorElement parent) : base(parent)
        {
            fakeRezistor = new ResistorElement(Double.PositiveInfinity);
            fakeCurrent = new CurrentSourceElement(parent.InitialCurrent);

            rEq = new LargeSignalResistorModel(fakeRezistor);
            iEq = new LargeSignalCurrentSourceModel(fakeCurrent);
        }

        private readonly LargeSignalResistorModel rEq;
        private readonly LargeSignalCurrentSourceModel iEq;

        private readonly ResistorElement fakeRezistor;
        private readonly CurrentSourceElement fakeCurrent;

        public override void Initialize(IEquationSystemBuilder builder)
        {
            fakeRezistor.Anode = Parent.Anode;
            fakeRezistor.Kathode = Parent.Kathode;
            
            fakeCurrent.Anode = Parent.Anode;
            fakeCurrent.Kathode = Parent.Kathode;

            iEq.Initialize(builder);
            rEq.Initialize(builder);
        }

        public void UpdateTimeDependentModel(SimulationContext context)
        {
            fakeRezistor.Resistance = context.Timestep / Parent.Capacity;
            fakeCurrent.Current = Parent.Capacity / context.Timestep * Vc;
            Vc = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equationSystem, SimulationContext context)
        {
            rEq.ApplyLinearModelValues(equationSystem, context);
            iEq.ApplyLinearModelValues(equationSystem, context);
        }
    }
}