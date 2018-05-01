using System.Composition;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Behaviors;
using NextGenSpice.LargeSignal.Devices;

namespace NextGenSpice.LargeSignal
{
    /// <summary>Class for creating <see cref="LargeSignalCircuitModel" /> from circuit definition.</summary>
    [Export(typeof(IAnalysisModelFactory<LargeSignalCircuitModel>))]
    public class LargeSignalAnalysisModelFactory : AnalysisModelFactory<LargeSignalCircuitModel>
    {
        public LargeSignalAnalysisModelFactory()
        {
            // register default models
            SetModel<ResistorDevice, LargeSignalResistor>(e => new LargeSignalResistor(e));
            SetModel<CurrentSourceDevice, LargeSignalCurrentSource>((e, ctx) =>
                new LargeSignalCurrentSource(e, (IInputSourceBehavior) ctx.GetParam(e.BehaviorParams)));
            SetModel<VoltageSourceDevice, LargeSignalVoltageSource>((e, ctx) =>
                new LargeSignalVoltageSource(e, (IInputSourceBehavior) ctx.GetParam(e.BehaviorParams)));
            SetModel<CapacitorDevice, LargeSignalCapacitor>(e => new LargeSignalCapacitor(e));
            SetModel<InductorDevice, LargeSignalInductor>(e => new LargeSignalInductor(e));
            SetModel<DiodeDevice, LargeSignalDiode>(e => new LargeSignalDiode(e));
            SetModel<BjtDevice, LargeSignalBjt>(e => new LargeSignalBjt(e));
            SetModel<VoltageControlledVoltageSourceDevice, LargeSignalVcvs>(e => new LargeSignalVcvs(e));
            SetModel<VoltageControlledCurrentSourceDevice, LargeSignalVccs>(e => new LargeSignalVccs(e));
            SetModel<CurrentControlledVoltageSourceDevice, LargeSignalCcvs>((e,ctx) => new LargeSignalCcvs(e, (LargeSignalVoltageSource) ctx.GetModel(e.Ampermeter)));
            SetModel<CurrentControlledCurrentSourceDevice, LargeSignalCccs>((e,ctx) => new LargeSignalCccs(e, (LargeSignalVoltageSource) ctx.GetModel(e.Ampermeter)));

            SetModel<SubcircuitDevice, LargeSignalSubcircuit>((e, ctx) =>
                new LargeSignalSubcircuit(e, e.Devices.Select(ctx.GetModel).Cast<ILargeSignalDevice>()));

            // Input source behaviors
            SetParam<ConstantBehaviorParams>(def => new ConstantSourceBehavior(def));
            SetParam<PulseBehaviorParams>(def => new PulseSourceBehavior(def));
            SetParam<PieceWiseLinearBehaviorParams>(def => new PieceWiseLinearSourceBehavior(def));
            SetParam<SinusoidalBehaviorParams>(def => new SinusioidalSourceBehavior(def));
            SetParam<ExponentialBehaviorParams>(def => new ExponentialSourceBehavior(def));
            SetParam<SffmBehaviorParams>(def => new SffmSourceBehavior(def));
            SetParam<AmBehaviorParams>(def => new AmSourceBehavior(def));
        }


        /// <summary>Factory method for creating the actual instance of the analysis model.</summary>
        /// <param name="context">Current instantiation context.</param>
        /// <returns></returns>
        protected override LargeSignalCircuitModel Instantiate(
            IModelInstantiationContext<LargeSignalCircuitModel> context)
        {
            var devices = context.CircuitDefinition.Devices
                .Select(context.GetModel).Cast<ILargeSignalDevice>().ToList();

            return new LargeSignalCircuitModel(context.CircuitDefinition.InitialVoltages, devices);
        }
    }
}