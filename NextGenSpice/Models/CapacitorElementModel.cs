using NextGenSpice.Elements;
using NextGenSpice.Equations;

namespace NextGenSpice.Models
{
    public class CapacitorElementModel : ITimeDependentCircuitModelElement
    {
        private readonly CapacitorElement parent;
        private double Vc;

        public CapacitorElementModel(CapacitorElement parent)
        {
            this.parent = parent;

            r_eq = new RezistorElement(double.PositiveInfinity);
            i_eq = new CurrentSourceElement(parent.InitialCurrent);
        }

        private readonly RezistorElement r_eq;
        private readonly CurrentSourceElement i_eq;

        public void Initialize()
        {
            r_eq.Anode = parent.Anode;
            r_eq.Kathode = parent.Kathode;

            // equivalent current has reverse polarity
            i_eq.Anode = parent.Anode;
            i_eq.Kathode = parent.Kathode;
        }

        public void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            // do nothing
        }

        public void UpdateTimeDependentModel(SimulationContext context)
        {
            r_eq.Resistance = context.Timestep / parent.Capacity;
            i_eq.Current = parent.Capacity / context.Timestep * Vc;
            Vc = parent.Anode.Voltage - parent.Kathode.Voltage;
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equationSystem, SimulationContext context)
        {
            r_eq.ApplyLinearModelValues(equationSystem, context);
            i_eq.ApplyLinearModelValues(equationSystem, context);
        }
    }
}