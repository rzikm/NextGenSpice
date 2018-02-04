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
            var v_be = Voltage(Base, Emitter, context);
            var v_bc = Voltage(Base, Collector, context);

            var i_f = DiodeCurrent(i_s, v_be, n_f);
            var i_r = DiodeCurrent(i_s, v_bc, n_r);

            var q1 = 1 / (1 - v_bc / v_af - v_be / v_ar);
            var q2 = i_f / i_kf + i_r / i_kr;

            var q_b = q1 / 2 * (1 + Math.Sqrt(1 + 4 * q2));

            var i_be = i_f / b_f + DiodeCurrent(i_se, v_be, n_e);
            var g_bei = i_s * Slope(v_be, n_f) / (n_f * v_t * b_f);
            var g_be = g_bei + i_se * Slope(v_be, n_e) / (n_e * v_t);

            var i_bc = i_r / b_r + DiodeCurrent(i_sc, v_bc, n_c);
            var g_bci = i_s * Slope(v_bc, n_r) / (n_r * v_t * b_r);
            var g_bc = g_bci + i_sc * Slope(v_bc, n_c) / (n_c * v_t);

            var i_t = (i_f - i_r) / q_b;

            var g_if = g_bei * b_f;
            var d_qbd_vbe = q1 * (q_b / v_ar + g_if / (i_kf * Math.Sqrt(1 + 4 * q2)));

            var g_ir = g_bci * b_r;
            var d_qbd_vbc = q1 * (q_b / v_af + g_ir / (i_kr * Math.Sqrt(1 + 4 * q2)));

            var g_mf = 1 / q_b * (g_if - i_t * d_qbd_vbe);
            var g_mr = 1 / q_b * (-g_ir - i_t * d_qbd_vbc);

            var i_beeq = i_be - g_be * v_be;
            var i_bceq = i_bc - g_bc * v_bc;
            var i_ceeq = i_t - g_mf * v_be + g_mr * v_bc;

            equations.AddMatrixEntry(Base, Base, g_be + g_bc);
            equations.AddMatrixEntry(Base, Collector, -g_bc);
            equations.AddMatrixEntry(Base, Emitter, -g_be);
            equations.AddRightHandSideEntry(Base, -i_beeq - i_bceq);

            equations.AddMatrixEntry(Collector, Base, -g_bc + g_mf - g_mr);
            equations.AddMatrixEntry(Collector, Collector, g_bc + g_mf);
            equations.AddMatrixEntry(Collector, Emitter, -g_mf);
            equations.AddRightHandSideEntry(Collector, i_bceq - i_ceeq);

            equations.AddMatrixEntry(Emitter, Base, g_be - g_mf + g_mr);
            equations.AddMatrixEntry(Emitter, Collector, -g_mr);
            equations.AddMatrixEntry(Emitter, Emitter, g_be + g_mr);
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
    }
}