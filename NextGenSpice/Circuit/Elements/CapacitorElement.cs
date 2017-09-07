namespace NextGenSpice.Circuit
{
    public class CapacitorElement : TwoNodeCircuitElement, INonlinearCircuitElement
    {
        public double Capacity { get; }
        
        private readonly RezistorElement r_eq;
        private readonly CurrentSourceElement i_eq;

        private double Vc;


        public CapacitorElement(double capacity, double initialCurrent)
        {
            this.Capacity = capacity;
            r_eq = new RezistorElement(double.PositiveInfinity);
            i_eq = new CurrentSourceElement(initialCurrent);
        }
        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            if (r_eq.Anode == null)
                Initialize();

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
            if (context.Time == 0) return; // model them as ideal open circuit
            r_eq.Resistance = context.Timestep / Capacity;
            i_eq.Current = Capacity / context.Timestep * Vc;
            Vc = Anode.Voltage - Kathode.Voltage;
        }
    }
}