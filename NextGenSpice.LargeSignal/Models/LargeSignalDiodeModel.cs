using System;
using System.Xml.Serialization;
using NextGenSpice.Core;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.LargeSignal.NumIntegration;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalDiodeModel : TwoNodeLargeSignalModel<DiodeElement>
    {
        private readonly double vt;
        private readonly double smallBiasTreshold;
        private readonly double capacitanceTreshold;
        private double gmin;

        private readonly DiodeModelParams param;

        public double Voltage { get; private set; }
        public double Current { get; private set; }

        private IIntegrationMethod IntegrationMethod { get; }

        private double ic;
        private double vc;
        private int capacitorBranch;


        private double lastTime;
        private string logstring;

        public LargeSignalDiodeModel(DiodeElement parent) : base(parent)
        {
//            IntegrationMethod = new AdamsMoultonIntegrationMethod(2);
            IntegrationMethod = new GearIntegrationMethod(4);
//            IntegrationMethod = new TrapezoidalIntegrationMethod();
//            IntegrationMethod = new BackwardEulerIntegrationMethod();
//            IntegrationMethod = new AdamsMoultonIntegrationMethod(3);

            param = parent.param;

            vt = param.EmissionCoefficient * PhysicalConstants.Boltzmann * PhysicalConstants.CelsiusToKelvin(param.Temperature) /
                 PhysicalConstants.ElementaryCharge;

            smallBiasTreshold = -5 * vt;
            capacitanceTreshold = param.ForwardBiasDepletionCapacitanceCoefficient * param.JunctionPotential;
        }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder, ISimulationContext context)
        {
            base.RegisterAdditionalVariables(builder, context);
            capacitorBranch = builder.AddVariable();
            gmin = param.MinimalResistance ?? context.CircuitParameters.MinimalResistance;
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            ApplyLinearizedModel(equations, context, context.GetSolutionForVariable(Parent.Anode) - context.GetSolutionForVariable(Parent.Kathode) - param.SeriesResistance * Current);
        }

        public override void ApplyInitialCondition(IEquationEditor equations, ISimulationContext context)
        {
            var vd = context.GetSolutionForVariable(Parent.Anode) - context.GetSolutionForVariable(Parent.Kathode);
            ApplyLinearizedModel(equations, context, vd == 0 ? param.Vd : vd - param.SeriesResistance * Current);
        }

        public override bool IsNonlinear => true;
        public override bool IsTimeDependent => true;

        private void ApplyLinearizedModel(IEquationEditor equations, ISimulationContext context, double vd)
        {
            var (id, geq, cd) = GetModelValues(vd);
            var ieq = id - geq * vd;
            // Diode
            equations
                .AddConductance(Anode, Kathode, geq)
                .AddCurrent(Kathode, Anode, ieq)
                ;

            // Capacitor
//            var cieq = 0.0;
//            var cgeq = cd / context.TimeStep * 2;

            var (cgeq, cieq) = IntegrationMethod.GetEquivalents(cd / context.TimeStep);

            if (context.TimeStep > 0)
            {
//                cieq = cgeq * vc + ic;
                equations.AddMatrixEntry(capacitorBranch, Anode, cgeq);
                equations.AddMatrixEntry(capacitorBranch, Kathode, -cgeq);
                equations.AddRightHandSideEntry(capacitorBranch, cieq);
            }
            equations.AddMatrixEntry(Anode, capacitorBranch, +1);
            equations.AddMatrixEntry(Kathode, capacitorBranch, -1);
            equations.AddMatrixEntry(capacitorBranch, capacitorBranch, -1);

            Voltage = vd;
            Current = id + ic;

//            if (context.Time > lastTime && Name == "D1") // bias established -> advancing in time
//            {
//                                Console.WriteLine(logstring);
//                lastTime = context.Time;
//            }
//            logstring = $"{context.Time} {vc} {ic} {id} {geq} {cd} {ieq} {cgeq} {cieq}";
        }

        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            ic = context.GetSolutionForVariable(capacitorBranch);
            if (context.TimeStep == 0) // set initial condition for the capacitor
            {
                ic = Current;
            }
            vc = Voltage;

            IntegrationMethod.SetState(ic, vc);
        }

        private (double, double, double) GetModelValues(double vd)
        {
            var m = param.JunctionGradingCoefficient;
            var vj = param.JunctionPotential;
            var fc = param.ForwardBiasDepletionCapacitanceCoefficient;
            var tt = param.TransitTime;
            var iss = param.SaturationCurrent;
            var jc = param.JunctionCapacitance;
            var bv = param.ReverseBreakdownVoltage;

            double id, geq, cd;
            if (vd >= smallBiasTreshold)
            {
                id = iss * (Math.Exp(vd / vt) - 1) + vd * gmin;
                geq = iss * Math.Exp(vd / vt) / vt + gmin;
                //                cd = tt * geq / vt;
            }
            else if (vd > -bv)
            {
                id = -iss;
                geq = gmin;
                //                geq += -iss / vd;
                //                cd = -tt * iss / vd;
            }
            else
            {
                id = -iss * (Math.Exp(-(bv + vd) / vt) - 1 + bv / vt);
                geq = iss * Math.Exp(-(bv + vd) / vt) / vt;
                //                                geq = gmin;
                cd = 0;
            }


            cd = -tt * geq;


            if (vd < capacitanceTreshold)
            {
                cd += jc / Math.Pow(1 - vd / vj, m);
            }
            else
            {
                cd += jc / Math.Pow(1 - fc, 1 + m) * (1 - fc * (1 + m) + m * vd / vj);
            }

            return (id, geq, cd);
        }

    }
}