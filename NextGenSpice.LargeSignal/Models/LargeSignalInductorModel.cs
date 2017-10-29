using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalInductorModel : TwoNodeLargeSignalModel<InductorElement>, ITimeDependentLargeSignalDeviceModel
    {
        private readonly LargeSignalResistorModel r_eq;
        private readonly LargeSignalVoltageSourceModel v_eq;

        private readonly ResistorElement fakeRezistor;
        private readonly VoltageSourceElement fakeVoltage;

        private double Vc;

        public LargeSignalInductorModel(InductorElement parent) : base(parent)
        {
            fakeRezistor = new ResistorElement(0);
            fakeVoltage = new VoltageSourceElement(parent.InitialVoltage);

            r_eq = new LargeSignalResistorModel(fakeRezistor);
            v_eq = new LargeSignalVoltageSourceModel(fakeVoltage);
        }

        public void Initialize()
        {
            fakeRezistor.Anode = Parent.Anode;
            fakeRezistor.Kathode = Parent.Kathode;

            fakeVoltage.Anode = Parent.Anode;
            fakeVoltage.Kathode = Parent.Kathode;
        }
        
        public void UpdateTimeDependentModel(SimulationContext context)
        {
            fakeRezistor.Resistance = context.Timestep / Parent.Inductance;
            fakeVoltage.Voltage = Parent.Inductance / context.Timestep * Vc;
            Vc = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equationSystem, SimulationContext context)
        {
            if (Math.Abs(r_eq.Resistance) < Double.Epsilon)
            {
                 equationSystem.BindEquivalent(r_eq.Anode, r_eq.Kathode);
                return;
            }

            r_eq.ApplyLinearModelValues(equationSystem, context);
            // TODO: solve this
            //v_eq.ApplyLinearModelValues(equationSystem, context);
        }
    }
}