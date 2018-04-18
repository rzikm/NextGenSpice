using System.Composition;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Behaviors;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal
{
    /// <summary>Class for creating <see cref="LargeSignalCircuitModel" /> from circuit definition.</summary>
    [Export(typeof(IAnalysisModelFactory<LargeSignalCircuitModel>))]
    public class LargeSignalAnalysisModelFactory : AnalysisModelFactory<LargeSignalCircuitModel>
    {
        public LargeSignalAnalysisModelFactory()
        {
            // register default models
            SetModel<ResistorDevice, LargeSignalResistorModel>(e => new LargeSignalResistorModel(e));
            SetModel<CurrentSourceDevice, LargeSignalCurrentSourceModel>((e, ctx) =>
                new LargeSignalCurrentSourceModel(e, (IInputSourceBehavior) ctx.GetParam(e.BehaviorParams)));
            SetModel<VoltageSourceDevice, LargeSignalVoltageSourceModel>((e, ctx) =>
                new LargeSignalVoltageSourceModel(e, (IInputSourceBehavior) ctx.GetParam(e.BehaviorParams)));
            SetModel<CapacitorDevice, LargeSignalCapacitorModel>(e => new LargeSignalCapacitorModel(e));
            SetModel<InductorDevice, LargeSignalInductorModel>(e => new LargeSignalInductorModel(e));
            SetModel<DiodeDevice, LargeSignalDiodeModel>(e => new LargeSignalDiodeModel(e));
            SetModel<BjtDevice, LargeSignalBjtModel>(e => new LargeSignalBjtModel(e));
            SetModel<VoltageControlledVoltageSourceDevice, LargeSignalVcvsModel>(e => new LargeSignalVcvsModel(e));
            SetModel<VoltageControlledCurrentSourceDevice, LargeSignalVccsModel>(e => new LargeSignalVccsModel(e));

            SetModel<SubcircuitDevice, LargeSignalSubcircuitModel>((e, ctx) =>
                new LargeSignalSubcircuitModel(e, e.Devices.Select(ctx.GetModel).Cast<ILargeSignalDeviceModel>()));

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
                .Select(context.GetModel).Cast<ILargeSignalDeviceModel>().ToList();

            return new LargeSignalCircuitModel(context.CircuitDefinition.InitialVoltages, devices);
        }
    }
}