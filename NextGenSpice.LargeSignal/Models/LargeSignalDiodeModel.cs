using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalDiodeModel : TwoNodeLargeSignalModel<DiodeElement>, INonlinearLargeSignalDeviceModel
    {
        private readonly LargeSignalResistorModel rEq;
        private readonly LargeSignalCurrentSourceModel iEq;

        private readonly ResistorElement fakeRezistor;
        private readonly CurrentSourceElement fakeCurrent;
        public LargeSignalDiodeModel(DiodeElement parent) : base(parent)
        {
            fakeRezistor = new ResistorElement(0);
            fakeCurrent = new CurrentSourceElement(0);

            Vd = parent.param.Vd;
            rEq = new LargeSignalResistorModel(fakeRezistor);
            iEq = new LargeSignalCurrentSourceModel(fakeCurrent);

            RecomputeLinearCircuit();
        }

        public double Vd { get; private set; }

        public void ApplyNonlinearModelValues(IEquationSystem equationSystem, SimulationContext context)
        {
            rEq.ApplyLinearModelValues(equationSystem, context);
            iEq.ApplyLinearModelValues(equationSystem, context);
        }

        public void UpdateNonlinearModel(SimulationContext context)
        {
            Vd = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
            RecomputeLinearCircuit();
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);

            fakeRezistor.Anode = Parent.Anode;
            fakeRezistor.Kathode = Parent.Kathode;

            // equivalent current has reverse polarity
            fakeCurrent.Anode = Parent.Kathode;
            fakeCurrent.Kathode = Parent.Anode;

            rEq.Initialize(builder);
            iEq.Initialize(builder);
        }

        public void RecomputeLinearCircuit()
        {
            var id = Parent.param.IS * (Math.Exp(Vd / Parent.param.Vt) - 1);
            var geq = (Parent.param.IS / Parent.param.Vt * Math.Exp(Vd / Parent.param.Vt));
            var ieq = id - geq * Vd;

            fakeRezistor.Resistance = 1 / geq;
            fakeCurrent.Current = ieq;
        }
    }
}