using System.Collections.Generic;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Numerics.Equations.Eq;

namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Large signal model for <see cref="ResistorDevice" /> device.</summary>
    public class LargeSignalResistor : TwoTerminalLargeSignalDevice<ResistorDevice>
    {
        private ConductanceStamper stamper;
        private VoltageProxy voltage;
        public LargeSignalResistor(ResistorDevice definitionDevice) : base(definitionDevice)
        {
            stamper = new ConductanceStamper();
            voltage = new VoltageProxy();
        }

        /// <summary>Specifies how often the model should be updated.</summary>
        public override ModelUpdateMode UpdateMode => ModelUpdateMode.NoUpdate;

        /// <summary>Resistance of the device in ohms.</summary>
        public double Resistance => DefinitionDevice.Resistance;

        /// <summary>Performs necessary initialization of the device, like mapping to the equation system.</summary>
        /// <param name="adapter">The equation system builder.</param>
        /// <param name="context">Context of current simulation.</param>
        public override void Initialize(IEquationSystemAdapter adapter, ISimulationContext context)
        {
            stamper.Register(adapter, Anode, Cathode);
            voltage.Register(adapter, Anode, Cathode);
        }

        /// <summary>
        ///     Applies device impact on the circuit equation system. If behavior of the device is nonlinear, this method is
        ///     called once every Newton-Raphson iteration.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void ApplyModelValues(ISimulationContext context)
        {
            stamper.Stamp(1/Resistance);
        }

        /// <summary>
        ///     Notifies model class that DC bias for given timepoint is established. This method can be used for processing
        ///     circuit equation solution for current timepoint.
        /// </summary>
        /// <param name="context">Context of current simulation.</param>
        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            Voltage = voltage.GetValue();
            Current = Voltage / Resistance;
        }
    }

    public class ConductanceStamper
    {
        private IEquationSystemCoefficientProxy n11;
        private IEquationSystemCoefficientProxy n12;
        private IEquationSystemCoefficientProxy n22;
        private IEquationSystemCoefficientProxy n21;
        public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
        {
            n11 = adapter.GetMatrixCoefficientProxy(anode, anode);
            n12 = adapter.GetMatrixCoefficientProxy(anode, cathode);
            n21 = adapter.GetMatrixCoefficientProxy(cathode, anode);
            n22 = adapter.GetMatrixCoefficientProxy(cathode, cathode);
        }

        public void Stamp(double value)
        {
            n11.Add(value);
            n12.Add(-value);
            n22.Add(value);
            n21.Add(-value);
        }
    }

    public class VoltageProxy
    {
        private IEquationSystemSolutionProxy anode;
        private IEquationSystemSolutionProxy cathode;

        public void Register(IEquationSystemAdapter adapter, int anode, int cathode)
        {
            this.anode = adapter.GetSolutionProxy(anode);
            this.cathode = adapter.GetSolutionProxy(cathode);
        }

        public double GetValue()
        {
            return anode.GetValue() - cathode.GetValue();
        }
    }
}