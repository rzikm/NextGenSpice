using System;
using NextGenSpice.Core;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalDiodeModel : TwoNodeLargeSignalModel<DiodeElement>
    {
        private double vt;
        private double smallBiasTreshold;
        private double gmin;

        private DiodeModelParams param;

        private double gEq;
        private double iEq;

        public LargeSignalDiodeModel(DiodeElement parent) : base(parent)
        {
            param = parent.param;

            vt = param.EmissionCoefficient * PhysicalConstants.Boltzmann * PhysicalConstants.CelsiusToKelvin(param.Temperature) /
                 PhysicalConstants.ElementaryCharge;

            smallBiasTreshold = -5 * vt;
        }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.RegisterAdditionalVariables(builder, context);
            gmin = param.MinimalResistance ?? context.CircuitParameters.MinimalResistance;
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            ApplyLinearizedModel(equations, context.GetSolutionForVariable(Parent.Anode) - context.GetSolutionForVariable(Parent.Kathode));
        }

        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            var vd = context.GetSolutionForVariable(Parent.Anode) - context.GetSolutionForVariable(Parent.Kathode);
            ApplyLinearizedModel(equations, vd == 0 ? param.Vd : vd);
        }

        public override bool IsNonlinear => true;
        public override bool IsTimeDependent  => false;

        private void ApplyLinearizedModel(IEquationEditor equations, double vd)
        {
            var (id, geq) = GetModelValues(vd);
            var ieq = id - geq * vd;

            iEq = ieq;
            gEq = geq;

            equations
                .AddConductance(Anode, Kathode, gEq)
                .AddCurrent(Kathode, Anode, iEq);
        }

        private (double, double) GetModelValues(double vd)
        {
            double id, geq;
            if (vd >= smallBiasTreshold)
            {
                id = param.SaturationCurrent * (Math.Exp(vd / vt) - 1) + vd * gmin;
                geq = param.SaturationCurrent * Math.Exp(vd / vt) / vt + gmin;
            }
            else if (vd > -param.ReverseBreakdownVoltage)
            {
                id = -param.SaturationCurrent;
                geq = gmin;
            }
            else if (vd == -param.ReverseBreakdownVoltage)
            {
                id = -param.ReverseBreakdownCurrent;
                geq = gmin;
            }
            else
            {
                id = -param.SaturationCurrent * (Math.Exp(-(param.ReverseBreakdownVoltage + vd) / vt) - 1 +
                                                 param.ReverseBreakdownVoltage / vt);
                geq = param.SaturationCurrent * Math.Exp(-(param.ReverseBreakdownVoltage + vd) / vt) / vt;
            }

            return (id, geq);
        }

    }
}