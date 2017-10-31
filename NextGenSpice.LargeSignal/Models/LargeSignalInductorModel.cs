using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalInductorModel : TwoNodeLargeSignalModel<InductorElement>, ITimeDependentLargeSignalDeviceModel
    {
        private readonly LargeSignalResistorModel rEq;
        private readonly LargeSignalVoltageSourceModel vEq;

        private readonly ResistorElement fakeRezistor;
        private readonly VoltageSourceElement fakeVoltage;

        private double vc;

        

        public LargeSignalInductorModel(InductorElement parent) : base(parent)
        {
            fakeRezistor = new ResistorElement(0);
            fakeVoltage = new VoltageSourceElement(parent.InitialVoltage);

            rEq = new LargeSignalResistorModel(fakeRezistor);
            vEq = new LargeSignalVoltageSourceModel(fakeVoltage);
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);
            fakeRezistor.Anode = Parent.Anode;
            fakeRezistor.Kathode = Parent.Kathode;

            fakeVoltage.Anode = Parent.Anode;
            fakeVoltage.Kathode = Parent.Kathode;

            vEq.Initialize(builder);
            rEq.Initialize(builder);
        }
        
        public void UpdateTimeDependentModel(SimulationContext context)
        {
            fakeRezistor.Resistance = context.Timestep / Parent.Inductance;
            fakeVoltage.Voltage = Parent.Inductance / context.Timestep * vc;
            vc = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equationSystem, SimulationContext context)
        {
            if (Math.Abs(rEq.Resistance) < Double.Epsilon)
            {
                //TODO: Use different method of calculating?
                 equationSystem.BindEquivalent(rEq.Anode, rEq.Kathode);
                return;
            }

            rEq.ApplyLinearModelValues(equationSystem, context);
            vEq.ApplyLinearModelValues(equationSystem, context);
        }
    }
}