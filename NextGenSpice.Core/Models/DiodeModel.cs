using System;
using NextGenSpice.Elements;
using NextGenSpice.Equations;

namespace NextGenSpice.Models
{
    public class DiodeModel : INonlinearLargeSignalDeviceModel
    {
        private readonly DiodeElement parent;
        private readonly Resistor r_eq;
        private readonly CurrentSource i_eq;

        public DiodeModel(DiodeElement parent)
        {
            Vd = parent.param.Vd;
            r_eq = new Resistor(0);
            i_eq = new CurrentSource(0);
            this.parent = parent;

            RecomputeLinearCircuit();
        }

        public double Vd { get; private set; }

        public void ApplyLinearModelValues(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            Initialize();
        }

        public void ApplyNonlinearModelValues(IEquationSystem equationSystem, SimulationContext context)
        {
            r_eq.ApplyLinearModelValues(equationSystem, context);
            i_eq.ApplyLinearModelValues(equationSystem, context);
        }

        public void UpdateNonlinearModel(SimulationContext context)
        {
            Vd = parent.Anode.Voltage - parent.Kathode.Voltage;
            RecomputeLinearCircuit();
        }

        public void Initialize()
        {
            r_eq.Anode = parent.Anode;
            r_eq.Kathode = parent.Kathode;

            // equivalent current has reverse polarity
            i_eq.Anode = parent.Kathode;
            i_eq.Kathode = parent.Anode;
        }

        public void RecomputeLinearCircuit()
        {
            var Id = parent.param.IS * (Math.Exp(Vd / parent.param.Vt) - 1);
            var Geq = (parent.param.IS / parent.param.Vt * Math.Exp(Vd / parent.param.Vt));
            var Ieq = Id - Geq * Vd;

            r_eq.Resistance = 1 / Geq;
            i_eq.Current = Ieq;
        }
    }
}