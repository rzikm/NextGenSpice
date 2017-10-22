using System;
using NextGenSpice.Elements;
using NextGenSpice.Equations;

namespace NextGenSpice.Models
{
    public class LargeSignalInductorModel : ITimeDependentLargeSignalDeviceModel
    {
        private readonly InductorElement parent;

        private readonly LargeSignalResistorModel r_eq;
        private readonly LargeSignalCurrentSourceModel i_eq;

        private double Vc;

        public LargeSignalInductorModel(InductorElement parent)
        {
            this.parent = parent;

            r_eq = new LargeSignalResistorModel(0);
            i_eq = new LargeSignalCurrentSourceModel(parent.InitialVoltage);
        }

        public void Initialize()
        {
            r_eq.Anode = parent.Anode;
            r_eq.Kathode = parent.Kathode;

            // equivalent current has reverse polarity
            i_eq.Anode = parent.Anode;
            i_eq.Kathode = parent.Kathode;
        }
        
        public void UpdateTimeDependentModel(SimulationContext context)
        {
            r_eq.Resistance = context.Timestep / parent.Inductance;
            i_eq.Current = parent.Inductance / context.Timestep * Vc;
            Vc = parent.Anode.Voltage - parent.Kathode.Voltage;
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equationSystem, SimulationContext context)
        {
            if (Math.Abs(r_eq.Resistance) < Double.Epsilon)
            {
                 equationSystem.BindEquivalent(r_eq.Anode.Id, r_eq.Kathode.Id);
                return;
            }

            r_eq.ApplyLinearModelValues(equationSystem, context);
            i_eq.ApplyLinearModelValues(equationSystem, context);
        }
    }
}