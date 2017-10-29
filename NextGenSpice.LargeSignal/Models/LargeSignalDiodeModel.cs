using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalDiodeModel : TwoNodeLargeSignalModel<DiodeElement>, INonlinearLargeSignalDeviceModel
    {
        private readonly LargeSignalResistorModel r_eq;
        private readonly LargeSignalCurrentSourceModel i_eq;

        private readonly ResistorElement fakeRezistor;
        private readonly CurrentSourceElement fakeCurrent;
        public LargeSignalDiodeModel(DiodeElement parent) : base(parent)
        {
            fakeRezistor = new ResistorElement(0);
            fakeCurrent = new CurrentSourceElement(0);

            Vd = parent.param.Vd;
            r_eq = new LargeSignalResistorModel(fakeRezistor);
            i_eq = new LargeSignalCurrentSourceModel(fakeCurrent);

            RecomputeLinearCircuit();
        }

        public double Vd { get; private set; }
        
        public void ApplyNonlinearModelValues(IEquationSystem equationSystem, SimulationContext context)
        {
            r_eq.ApplyLinearModelValues(equationSystem, context);
            i_eq.ApplyLinearModelValues(equationSystem, context);
        }

        public void UpdateNonlinearModel(SimulationContext context)
        {
            Vd = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
            RecomputeLinearCircuit();
        }

        public void Initialize()
        {
            fakeRezistor.Anode = Parent.Anode;
            fakeRezistor.Kathode = Parent.Kathode;

            // equivalent current has reverse polarity
           fakeCurrent.Anode = Parent.Kathode;
            fakeCurrent.Kathode = Parent.Anode;
        }

        public void RecomputeLinearCircuit()
        {
            var Id = Parent.param.IS * (Math.Exp(Vd / Parent.param.Vt) - 1);
            var Geq = (Parent.param.IS / Parent.param.Vt * Math.Exp(Vd / Parent.param.Vt));
            var Ieq = Id - Geq * Vd;

            fakeRezistor.Resistance = 1 / Geq;
            fakeCurrent.Current = Ieq;
        }
    }
}