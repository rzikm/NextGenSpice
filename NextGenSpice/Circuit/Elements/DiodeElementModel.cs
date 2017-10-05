using System;

namespace NextGenSpice.Circuit
{
    public class DiodeElementModel : INonlinearCircuitModelElement
    {
        private readonly DiodeElement parent;
        private readonly RezistorElement r_eq;
        private readonly CurrentSourceElement i_eq;

        public DiodeElementModel(DiodeElement parent)
        {
            Vd = parent.param.Vd;
            r_eq = new RezistorElement(0);
            i_eq = new CurrentSourceElement(0);
            this.parent = parent;

            RecomputeLinearCircuit();
        }

        public double Vd { get; private set; }

        public void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            Initialize();
        }

        public void ApplyToEquationsDynamic(IEquationSystem equationSystem, SimulationContext context)
        {
            r_eq.ApplyToEquationsPermanent(equationSystem, context);
            i_eq.ApplyToEquationsPermanent(equationSystem, context);
        }

        public void UpdateLinearizedModel(SimulationContext context)
        {
            Vd = parent.Anode.Voltage - parent.Kathode.Voltage;
            RecomputeLinearCircuit();
        }

        private void Initialize()
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