using NextGenSpice.Elements;
using NextGenSpice.Equations;
using NextGenSpice.Representation;

namespace NextGenSpice.Models
{
    public class LargeSignalCapacitorModel : ITimeDependentLargeSignalDeviceModel, IAnalysisDeviceModel<CapacitorElement>
    {
        private readonly CapacitorElement parent;
        private double Vc;

        public LargeSignalCapacitorModel(CapacitorElement parent)
        {
            this.parent = parent;

            r_eq = new LargeSignalResistorModel(double.PositiveInfinity);
            i_eq = new LargeSignalCurrentSourceModel(parent.InitialCurrent);
        }

        private readonly LargeSignalResistorModel r_eq;
        private readonly LargeSignalCurrentSourceModel i_eq;

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