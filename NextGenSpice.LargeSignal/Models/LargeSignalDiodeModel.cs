using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalDiodeModel : TwoNodeLargeSignalModel<DiodeElement>, INonlinearLargeSignalDeviceModel
    {
        private double iEq;
        private double gEq;

        public LargeSignalDiodeModel(DiodeElement parent) : base(parent)
        {
            Vd = parent.param.Vd;
            RecomputeLinearCircuit();
        }

        public double Vd { get; private set; }

        public void ApplyNonlinearModelValues(IEquationSystem equation, SimulationContext context)
        {
            equation
                .AddConductance(Anode, Kathode, gEq)
                .AddCurrent(Kathode, Anode, iEq);
        }

        public void UpdateNonlinearModel(SimulationContext context)
        {
            Vd = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
            RecomputeLinearCircuit();
        }

        public void RecomputeLinearCircuit()
        {
            var id = Parent.param.IS * (Math.Exp(Vd / Parent.param.Vt) - 1);
            var geq = (Parent.param.IS / Parent.param.Vt * Math.Exp(Vd / Parent.param.Vt));
            var ieq = id - geq * Vd;
            
            iEq = ieq;
            gEq = geq;
        }
    }
}