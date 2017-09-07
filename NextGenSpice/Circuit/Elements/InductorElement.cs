namespace NextGenSpice.Circuit
{
    public class InductorElement : TwoNodeCircuitElement, INonlinearCircuitElement
    {
        public double Inductance { get; }

        private readonly RezistorElement r_eq;
        private readonly CurrentSourceElement i_eq;

        private double Vc;


        public InductorElement(double inductance, double initialVoltage)
        {
            this.Inductance = inductance;
            r_eq = new RezistorElement(double.PositiveInfinity);
            i_eq = new CurrentSourceElement(initialVoltage);
        }
        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override void ApplyToEquationsPermanent(IEquationEditor equationSystem, SimulationContext context)
        {
            if (r_eq.Anode == null)
                Initialize();
            if (context.Time == 0)
            {
               // equationSystem.MergeNodes(Anode.Id, Kathode.Id);
                return;
            }
            
            r_eq.ApplyToEquationsPermanent(equationSystem, context);
            i_eq.ApplyToEquationsPermanent(equationSystem, context);
        }

        void Initialize()
        {
            r_eq.Anode = Anode;
            r_eq.Kathode = Kathode;

            // equivalent current has reverse polarity
            i_eq.Anode = Anode;
            i_eq.Kathode = Kathode;
        }

        public void UpdateLinearizedModel(SimulationContext context)
        {
            if (context.Time == 0) return; // model them as ideal closed circuit
            r_eq.Resistance = context.Timestep / Inductance;
            i_eq.Current = Inductance / context.Timestep * Vc;
            Vc = Anode.Voltage - Kathode.Voltage;
        }
    }
}