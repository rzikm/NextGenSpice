using System;
using NextGenSpice.Core;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Elements.Parameters;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>
    ///     Large signal model for <see cref="BjtElement" /> device.
    /// </summary>
    internal class LargeSignalBjtModel : LargeSignalModelBase<BjtElement>
    {
        private readonly double i_s;
        private readonly double i_se;
        private readonly double i_sc;

        private readonly double n_f;
        private readonly double n_r;
        private readonly double n_e;
        private readonly double n_c;

        private readonly double b_f;
        private readonly double b_r;

        private readonly double v_af;
        private readonly double v_ar;

        private readonly double i_kf;
        private readonly double i_kr;

        private readonly double v_t; // thermal voltage

        private double i_b;


        private double v_be;
        private double v_bc;

        private double i_f;
        private double i_r;

        private double q1;
        private double q2;

        private double q_b;

        private double i_be;
        private double g_bei;
        private double g_be;

        private double i_bc;
        private double g_bci;
        private double g_bc;

        private double i_t;

        private double g_if;
        private double d_qbd_vbe;

        private double g_ir;
        private double d_qbd_vbc;

        private double g_mf;
        private double g_mr;

        private double i_beeq;
        private double i_bceq;
        //            private double i_ceeq;


        private double g0;
        private double g_m;
        private double i_ceeq;




        public LargeSignalBjtModel(BjtElement definitionElement) : base(definitionElement)
        {
            v_t = PhysicalConstants.Boltzmann *
                 PhysicalConstants.CelsiusToKelvin(Parameters.NominalTemperature) /
                 PhysicalConstants.ElementaryCharge;

            i_s = Parameters.SaturationCurrent;
            i_se = Parameters.EmitterSaturationCurrent;
            i_sc = Parameters.CollectorSaturationCurrent;

            n_f = Parameters.ForwardEmissionCoefficient;
            n_r = Parameters.ReverseEmissionCoefficient;
            n_e = Parameters.EmitterSaturationCoefficient;
            n_c = Parameters.CollectorSaturationCoefficient;

            b_f = Parameters.ForwardBeta;
            b_r = Parameters.ReverseBeta;

            v_af = Parameters.ForwardEarlyVoltage;
            v_ar = Parameters.ReverseEarlyVoltage;

            i_kf = Parameters.ForwardCurrentCorner;
            i_kr = Parameters.ReverseCurrentCorner;
        }

        /// <summary>
        ///     Node connected to collector terminal of the transistor.
        /// </summary>
        public int Collector => DefinitionElement.Collector;

        /// <summary>
        ///     Node connected to base terminal of the transistor.
        /// </summary>
        public int Base => DefinitionElement.Base;

        /// <summary>
        ///     Node connected to emitter terminal of the transistor.
        /// </summary>
        public int Emitter => DefinitionElement.Emitter;

        /// <summary>
        ///     Node connected to substrate terminal of the transistor.
        /// </summary>
        public int Substrate => DefinitionElement.Substrate;

        /// <summary>
        ///     Set of parameters for this device model.
        /// </summary>
        public BjtModelParams Parameters => DefinitionElement.Parameters;

        /// <summary>
        ///     If true, the device behavior is not constant over time and the
        ///     <see cref="ILargeSignalDeviceModel.ApplyModelValues" /> function is called
        ///     every timestep.
        /// </summary>
        public override bool IsTimeDependent => false;

        /// <summary>
        ///     If true, the device behavior is not linear is not constant and the
        ///     <see cref="ILargeSignalDeviceModel.ApplyModelValues" /> function is
        ///     called every iteration during nonlinear solving.
        /// </summary>
        public override bool IsNonlinear => true;

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            // calculate values according to Gummel-Poon model
            v_be = Voltage(Base, Emitter, context);
            v_bc = Voltage(Base, Collector, context);

            i_f = DiodeCurrent(i_s, v_be, n_f);
            i_r = DiodeCurrent(i_s, v_bc, n_r);

            q1 = 1 / (1 - v_bc / v_af - v_be / v_ar);
            q2 = i_f / i_kf + i_r / i_kr;

            q_b = q1 / 2 * (1 + Math.Sqrt(1 + 4 * q2));

            i_be = i_f / b_f + DiodeCurrent(i_se, v_be, n_e);
            g_bei = i_s * Slope(v_be, n_f) / (n_f * v_t * b_f);
            g_be = g_bei + i_se * Slope(v_be, n_e) / (n_e * v_t);

            i_bc = i_r / b_r + DiodeCurrent(i_sc, v_bc, n_c);
            g_bci = i_s * Slope(v_bc, n_r) / (n_r * v_t * b_r);
            g_bc = g_bci + i_sc * Slope(v_bc, n_c) / (n_c * v_t);

            i_t = (i_f - i_r) / q_b;

            g_if = g_bei * b_f;
            d_qbd_vbe = q1 * (q_b / v_ar + g_if / (i_kf * Math.Sqrt(1 + 4 * q2)));

            g_ir = g_bci * b_r;
            d_qbd_vbc = q1 * (q_b / v_af + g_ir / (i_kr * Math.Sqrt(1 + 4 * q2)));

            g_mf = 1 / q_b * (g_if - i_t * d_qbd_vbe);
            g_mr = 1 / q_b * (-g_ir - i_t * d_qbd_vbc);

            i_beeq = i_be - g_be * v_be;
            i_bceq = i_bc - g_bc * v_bc;
            //             i_ceeq = i_t - g_mf * v_be + g_mr * v_bc;


            g0 = 1 / q_b * (g_ir + i_t * d_qbd_vbc);
            g_m = 1 / q_b * (g_if - i_t * d_qbd_vbe) - g0;
            double v_ce = Voltage(Collector, Emitter, context);
            i_ceeq = i_t - g_m * v_be - g0 * v_ce;

            i_b = i_be + i_bc;

            //            equations.AddMatrixEntry(Base, Base, g_be + g_bc);
            //            equations.AddMatrixEntry(Base, Collector, -g_bc);
            //            equations.AddMatrixEntry(Base, Emitter, -g_be);
            //
            //            equations.AddMatrixEntry(Collector, Base, -g_bc + g_mf - g_mr);
            //            equations.AddMatrixEntry(Collector, Collector, g_bc + g_mf);
            //            equations.AddMatrixEntry(Collector, Emitter, -g_mf);
            //
            //            equations.AddMatrixEntry(Emitter, Base, -g_be - g_mf + g_mr);
            //            equations.AddMatrixEntry(Emitter, Collector, -g_mr);
            //            equations.AddMatrixEntry(Emitter, Emitter, g_be + g_mr);

            equations.AddMatrixEntry(Base, Base, g_be + g_bc);
            equations.AddMatrixEntry(Base, Collector, -g_bc);
            equations.AddMatrixEntry(Base, Emitter, -g_be);

            equations.AddMatrixEntry(Collector, Base, -g_bc + g_m);
            equations.AddMatrixEntry(Collector, Collector, g_bc + g0);
            equations.AddMatrixEntry(Collector, Emitter, -g0 - g_m);

            equations.AddMatrixEntry(Emitter, Base, -g_be - g_m);
            equations.AddMatrixEntry(Emitter, Collector, -g0);
            equations.AddMatrixEntry(Emitter, Emitter, g_be + g_m + g0);

            equations.AddRightHandSideEntry(Base, -i_beeq - i_bceq);
            equations.AddRightHandSideEntry(Collector, i_bceq - i_ceeq);
            equations.AddRightHandSideEntry(Emitter, i_beeq + i_ceeq);
        }

        private double DiodeCurrent(double saturationCurrent, double voltage, double emissionCoef)
        {
            return saturationCurrent * (Slope(voltage, emissionCoef) - 1);
        }

        private double Slope(double voltage, double emissionCoef)
        {
            return Math.Exp(voltage / (emissionCoef * v_t));
        }

        private double Voltage(int n1, int n2, ISimulationContext context)
        {
            return context.GetSolutionForVariable(n1) - context.GetSolutionForVariable(n2);
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution
        ///     for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
        }
    }
}