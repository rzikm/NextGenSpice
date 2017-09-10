namespace NextGenSpice.Circuit
{
    public class InductorElement : TwoNodeCircuitElement
    {
        public double Inductance { get; }
        public double InitialVoltage { get; }


        public InductorElement(double inductance, double initialVoltage = 0)
        {
            this.Inductance = inductance;
            InitialVoltage = initialVoltage;
        }
        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override ICircuitModelElement GetDcOperatingPointModel()
        {
            return new ShortCircuitModel(this);
        }

        public override ICircuitModelElement GetTransientModel()
        {
            return new InductoreElementModel(this);
        }
    }

    public class InductoreElementModel : ICircuitModelElement
    {
        private readonly InductorElement parent;

        private readonly RezistorElement r_eq;
        private readonly CurrentSourceElement i_eq;

        private double Vc;


        public InductoreElementModel(InductorElement parent)
        {
            this.parent = parent;


            r_eq = new RezistorElement(double.PositiveInfinity);
            i_eq = new CurrentSourceElement(parent.InitialVoltage);
        }

        public void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
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
            r_eq.Anode = parent.Anode;
            r_eq.Kathode = parent.Kathode;

            // equivalent current has reverse polarity
            i_eq.Anode = parent.Anode;
            i_eq.Kathode = parent.Kathode;
        }

        public void UpdateLinearizedModel(SimulationContext context)
        {
            if (context.Time == 0) return; // model them as ideal closed circuit
            r_eq.Resistance = context.Timestep / parent.Inductance;
            i_eq.Current = parent.Inductance / context.Timestep * Vc;
            Vc = parent.Anode.Voltage - parent.Kathode.Voltage;
        }
    }
}