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
        private readonly double bF;
        private readonly double bR;

        private readonly double iKf;
        private readonly double iKr;
        private readonly double iS;
        private readonly double iSc;
        private readonly double iSe;
        private readonly double nC;
        private readonly double nE;

        private readonly double nF;
        private readonly double nR;

        private readonly double vAf;
        private readonly double vAr;

        private readonly double vT; // thermal voltage

        private double iB;
        private double iBc;

        private double iBe;
        private double iC;
        private double iE;
        private double vBc;

        private double vBe;

        public LargeSignalBjtModel(BjtElement definitionElement) : base(definitionElement)
        {
            vT = PhysicalConstants.Boltzmann *
                  PhysicalConstants.CelsiusToKelvin(Parameters.NominalTemperature) /
                  PhysicalConstants.ElementaryCharge;

            iS = Parameters.SaturationCurrent;
            iSe = Parameters.EmitterSaturationCurrent;
            iSc = Parameters.CollectorSaturationCurrent;

            nF = Parameters.ForwardEmissionCoefficient;
            nR = Parameters.ReverseEmissionCoefficient;
            nE = Parameters.EmitterSaturationCoefficient;
            nC = Parameters.CollectorSaturationCoefficient;

            bF = Parameters.ForwardBeta;
            bR = Parameters.ReverseBeta;

            vAf = Parameters.ForwardEarlyVoltage;
            vAr = Parameters.ReverseEarlyVoltage;

            iKf = Parameters.ForwardCurrentCorner;
            iKr = Parameters.ReverseCurrentCorner;
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
        ///     Current flowing through the base terminal.
        /// </summary>
        public double BaseCurrent => iB;

        /// <summary>
        ///     Current flowing through the collector terminal.
        /// </summary>
        public double CollectorCurrent => iC;

        /// <summary>
        ///     Current flowing through the emitter terminal.
        /// </summary>
        public double EmitterCurrent => iE;

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is called
        ///     once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="equations">Current linearized circuit equation system.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            // calculate values according to Gummel-Poon model
            vBe = Voltage(Base, Emitter, context);
            vBc = Voltage(Base, Collector, context);

            var (gBe, gBc, gMf, gMr, iT) = CalculateModelValues();

            double iBeeq = iBe - gBe * vBe;
            double iBceq = iBc - gBc * vBc;
            double iCeeq = iT - gMf * vBe - gMr * vBc;

            iB = iBe + iBc;

            equations.AddMatrixEntry(Base, Base, gBe + gBc);
            equations.AddMatrixEntry(Base, Collector, -gBc);
            equations.AddMatrixEntry(Base, Emitter, -gBe);

            equations.AddMatrixEntry(Collector, Base, -gBc + gMf + gMr);
            equations.AddMatrixEntry(Collector, Collector, gBc - gMr);
            equations.AddMatrixEntry(Collector, Emitter, -gMf);

            equations.AddMatrixEntry(Emitter, Base, -gBe - gMf - gMr);
            equations.AddMatrixEntry(Emitter, Collector, +gMr);
            equations.AddMatrixEntry(Emitter, Emitter, gBe + gMf);


            equations.AddRightHandSideEntry(Base, -iBeeq - iBceq);
            equations.AddRightHandSideEntry(Collector, iBceq - iCeeq);
            equations.AddRightHandSideEntry(Emitter, iBeeq + iCeeq);
        }

        private (double g_be, double g_bc, double g_mf, double g_mr, double i_t) CalculateModelValues()
        {
            // for details see http://qucs.sourceforge.net/tech/node70.html

            double iF = DiodeCurrent(iS, vBe, nF);
            double iR = DiodeCurrent(iS, vBc, nR);

            double q1 = 1 / (1 - vBc / vAf - vBe / vAr);
            double q2 = iF / iKf + iR / iKr;

            double qB = q1 / 2 * (1 + Math.Sqrt(1 + 4 * q2));

            iBe = iF / bF + DiodeCurrent(iSe, vBe, nE);
            double gBei = iS * Slope(vBe, nF) / (nF * vT * bF);
            double gBe = gBei + iSe * Slope(vBe, nE) / (nE * vT);

            iBc = iR / bR + DiodeCurrent(iSc, vBc, nC);
            double gBci = iS * Slope(vBc, nR) / (nR * vT * bR);
            double gBc = gBci + iSc * Slope(vBc, nC) / (nC * vT);

            double iT = (iF - iR) / qB;

            double gIf = gBei * bF;
            double dQbdVbe = q1 * (qB / vAr + gIf / (iKf * Math.Sqrt(1 + 4 * q2)));

            double gIr = gBci * bR;
            double dQbdVbc = q1 * (qB / vAf + gIr / (iKr * Math.Sqrt(1 + 4 * q2)));

            double gMf = 1 / qB * (gIf - iT * dQbdVbe);
            double gMr = 1 / qB * (-gIr - iT * dQbdVbc);

            // calculate terminal currents
            iC = iT - 1 / bR * iR;
            iE = -iT - 1 / bF * iF;

            return (gBe, gBc, gMf, gMr, iT);
        }

        private double DiodeCurrent(double saturationCurrent, double voltage, double emissionCoef)
        {
            return saturationCurrent * (Slope(voltage, emissionCoef) - 1);
        }

        private double Slope(double voltage, double emissionCoef)
        {
            return Math.Exp(voltage / (emissionCoef * vT));
        }

        private double Voltage(int n1, int n2, ISimulationContext context)
        {
            return context.GetSolutionForVariable(n1) - context.GetSolutionForVariable(n2);
        }
    }
}