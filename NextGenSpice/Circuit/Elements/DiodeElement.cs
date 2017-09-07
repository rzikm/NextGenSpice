using System;

namespace NextGenSpice.Circuit
{
    public class DiodeElement : TwoNodeCircuitElement, INonlinearCircuitElement
    {

        private readonly DiodeModelParams param;
        private RezistorElement r_eq;
        private CurrentSourceElement i_eq;

        public Double Vd { get; private set; }

        public DiodeElement(DiodeModelParams param)
        {
            this.param = param;
            Vd = this.param.Vd;
            r_eq = new RezistorElement(0);
            i_eq = new CurrentSourceElement(0);
            RecomputeLinearCircuit();
        }

        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            Initialize();
        }

        public override void ApplyToEquationsDynamic(IEquationSystem equationSystem, SimulationContext context)
        {
            base.ApplyToEquationsDynamic(equationSystem, context);

            r_eq.ApplyToEquationsPermanent(equationSystem, context);
            i_eq.ApplyToEquationsPermanent(equationSystem, context);
        }

        public void UpdateLinearizedModel(SimulationContext context)
        {
            Vd = Anode.Voltage - Kathode.Voltage;
            RecomputeLinearCircuit();
        }

        void Initialize()
        {
            r_eq.Anode = Anode;
            r_eq.Kathode = Kathode;

            // equivalent current has reverse polarity
            i_eq.Anode = Kathode;
            i_eq.Kathode = Anode;
        }

        private void RecomputeLinearCircuit()
        {
            var Id = param.IS * (Math.Exp(Vd / param.Vt) - 1);
            var Geq = (param.IS / param.Vt * Math.Exp(Vd / param.Vt));
            var Ieq = Id - Geq * Vd;

            r_eq.Resistance = 1 / Geq;
            i_eq.Current = Ieq;

            Console.WriteLine($"Vd = {Vd}, Geq = {Geq:E2}, Ieq = {Ieq:E2}");

        }
    }

    public interface ICanonicalElement
    {
        void ApplyToEquationsPermanent(IEquationEditor equationSystem, SimulationContext context);
    }
}