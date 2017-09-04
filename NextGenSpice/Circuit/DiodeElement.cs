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

        public override void ApplyToEquations(ICircuitEquationSystem equationSystem)
        {
            if (r_eq.Anode == null)
                Initialize();
            r_eq.ApplyToEquations(equationSystem);
            i_eq.ApplyToEquations(equationSystem);
        }

        public void UpdateLinearizedModel()
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
}