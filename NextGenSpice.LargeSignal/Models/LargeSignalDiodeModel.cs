using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalDiodeModel : TwoNodeLargeSignalModel<DiodeElement>
    {
        private double gEq;
        private double iEq;

        public LargeSignalDiodeModel(DiodeElement parent) : base(parent)
        {
            Vd = parent.param.Vd;
        }

        public double Vd { get; private set; }


        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            ApplyLinearizedModel(equations, context.GetSolutionForVariable(Parent.Anode) - context.GetSolutionForVariable(Parent.Kathode));
        }



        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            var vd = context.GetSolutionForVariable(Parent.Anode) - context.GetSolutionForVariable(Parent.Kathode);
            ApplyLinearizedModel(equations, vd == 0 ? Vd : vd);
        }

        public override bool IsNonlinear => true;
        public override bool IsTimeDependent  => false;

        private void ApplyLinearizedModel(IEquationEditor equations, double vd)
        {
            var id = Parent.param.IS * (Math.Exp(vd / Parent.param.Vt) - 1);
            var geq = Parent.param.IS / Parent.param.Vt * Math.Exp(vd / Parent.param.Vt);
            var ieq = id - geq * vd;

            iEq = ieq;
            gEq = geq;

            equations
                .AddConductance(Anode, Kathode, gEq)
                .AddCurrent(Kathode, Anode, iEq);
        }
        
    }
}