using System.Linq;

namespace NextGenSpice.Circuit
{
    public class ShortCircuitModel : INonlinearCircuitModelElement
    {
        private readonly ICircuitDefinitionElement parent;

        public ShortCircuitModel(ICircuitDefinitionElement parent)
        {
            this.parent = parent;
        }
        public void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
        }

        public void UpdateLinearizedModel(SimulationContext context)
        {
        }

        public void ApplyToEquationsDynamic(IEquationSystem equationSystem, SimulationContext context)
        {
            equationSystem.BindEquivalent(parent.ConnectedNodes.Select(n => n.Id));
        }
    }

    public class OpenCircuitModel : ICircuitModelElement
    {
        private static OpenCircuitModel instance;

        public static OpenCircuitModel Instance
        {
            get { return instance ?? (instance = new OpenCircuitModel()); }
        }
        public void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            // do nothing -> no conductance from Anode to Kathode
        }
    }

    public class CapacitorElementModel : ICircuitModelElement
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
        public void ApplyToEquationsPermanent(IEquationSystemBuilder equationSystem, SimulationContext context)
        {
            if (r_eq.Anode == null)
                Initialize();

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
            if (context.Time == 0) return; // model them as ideal open circuit
            r_eq.Resistance = context.Timestep / parent.Capacity;
            i_eq.Current = parent.Capacity / context.Timestep * Vc;
            Vc = parent.Anode.Voltage - parent.Kathode.Voltage;
        }
    }

    public class CapacitorElement : TwoNodeCircuitElement
    {
        public double Capacity { get; }
        public double InitialCurrent { get; }

        public CapacitorElement(double capacity, double initialCurrent = 0)
        {
            this.Capacity = capacity;
            InitialCurrent = initialCurrent;
        }
        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }

        public override ICircuitModelElement GetDcOperatingPointModel()
        {
            return OpenCircuitModel.Instance;
        }

        public override ICircuitModelElement GetTransientModel()
        {
            return new CapacitorElementModel(this);
        }
    }
}