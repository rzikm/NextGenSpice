using System.Composition;
using System.Linq;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Behaviors;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal
{
    /// <summary>
    ///     Class for creating <see cref="LargeSignalCircuitModel" /> from circuit definition.
    /// </summary>
    [Export(typeof(IAnalysisModelFactory<LargeSignalCircuitModel>))]
    public class LargeSignalAnalysisModelFactory : AnalysisModelFactory<LargeSignalCircuitModel>
    {
        public LargeSignalAnalysisModelFactory()
        {
            // register default models
            SetModel<ResistorElement, LargeSignalResistorModel>(e => new LargeSignalResistorModel(e));
            SetModel<CurrentSourceElement, LargeSignalCurrentSourceModel>((e, ctx) =>
                new LargeSignalCurrentSourceModel(e, (IInputSourceBehavior) ctx.GetParam(e.BehaviorParams)));
            SetModel<VoltageSourceElement, LargeSignalVoltageSourceModel>((e, ctx) =>
                new LargeSignalVoltageSourceModel(e, (IInputSourceBehavior) ctx.GetParam(e.BehaviorParams)));
            SetModel<CapacitorElement, LargeSignalCapacitorModel>(e => new LargeSignalCapacitorModel(e));
            SetModel<InductorElement, LargeSignalInductorModel>(e => new LargeSignalInductorModel(e));
            SetModel<DiodeElement, LargeSignalDiodeModel>(e => new LargeSignalDiodeModel(e));
            SetModel<BjtElement, LargeSignalBjtModel>(e => new LargeSignalBjtModel(e));

            SetModel<SubcircuitElement, LargeSignalSubcircuitModel>((e, ctx) =>
                new LargeSignalSubcircuitModel(e, e.Elements.Select(ctx.GetModel).Cast<ILargeSignalDeviceModel>()));

            // Input source behaviors
            SetParam<ConstantBehaviorParams>(def => new ConstantSourceBehavior(def));
            SetParam<PulseBehaviorParams>(def => new PulseSourceBehavior(def));
            SetParam<PieceWiseLinearBehaviorParams>(def => new PieceWiseLinearSourceBehavior(def));
            SetParam<SinusoidalBehaviorParams>(def => new SinusioidalSourceBehavior(def));
            SetParam<ExponentialBehaviorParams>(def => new ExponentialSourceBehavior(def));
            SetParam<SffmBehaviorParams>(def => new SffmSourceBehavior(def));
            SetParam<AmBehaviorParams>(def => new AmSourceBehavior(def));
            SetParam<VoltageControlledBehaviorParams>(def => new VoltageControlledSourceBehavior(def));
        }


        /// <summary>
        ///     Factory method for creating the actual instance of the analysis model.
        /// </summary>
        /// <param name="context">Current instantiation context.</param>
        /// <returns></returns>
        protected override LargeSignalCircuitModel Instantiate(
            IModelInstantiationContext<LargeSignalCircuitModel> context)
        {
            var elements = context.CircuitDefinition.Elements
                .Select(context.GetModel).Cast<ILargeSignalDeviceModel>().ToList();

            return new LargeSignalCircuitModel(context.CircuitDefinition.InitialVoltages, elements);
        }
    }
}