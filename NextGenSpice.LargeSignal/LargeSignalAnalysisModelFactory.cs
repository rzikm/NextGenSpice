using System;
using System.Composition;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using NextGenSpice.Core.BehaviorParams;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Behaviors;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal
{
    [Export(typeof(IAnalysisModelFactory<LargeSignalCircuitModel>))]
    public class LargeSignalAnalysisModelFactory : AnalysisModelFactory<LargeSignalCircuitModel>
    {
        public LargeSignalAnalysisModelFactory()
        {
            RegisterDefaultModels();
        }

        private void RegisterDefaultModels()
        {
            SetModel<DiodeElement, LargeSignalDiodeModel>(e => new LargeSignalDiodeModel(e));
            SetModel<ResistorElement, LargeSignalResistorModel>(e => new LargeSignalResistorModel(e));
            SetModel<CurrentSourceElement, LargeSignalCurrentSourceModel>((e, ctx) => new LargeSignalCurrentSourceModel(e, (IInputSourceBehavior) ctx.GetParam(e.BehaviorParams)));
            SetModel<VoltageSourceElement, LargeSignalVoltageSourceModel>((e, ctx) => new LargeSignalVoltageSourceModel(e, (IInputSourceBehavior) ctx.GetParam(e.BehaviorParams)));
            SetModel<CapacitorElement, LargeSignalCapacitorModel>(e => new LargeSignalCapacitorModel(e));
            SetModel<InductorElement, LargeSignalInductorModel>(e => new LargeSignalInductorModel(e));
            SetModel<SubcircuitElement, LargeSignalSubcircuitModel>((e, ctx) => new LargeSignalSubcircuitModel(e, e.Elements.Select(ctx.GetModel).Cast<ILargeSignalDeviceModel>()));

            // TODO: Autoregister using MEF?
            // Input source behaviors
            SetParam<ConstantBehaviorParams>((def, ctx) => new ConstantSourceBehavior(def));
            SetParam<PulseBehaviorParams>((def, ctx) => new PulseSourceBehavior(def));
            SetParam<PieceWiseLinearBehaviorParams>((def, ctx) => new PieceWiseLinearSourceBehavior(def));
            SetParam<SinusoidalBehaviorParams>((def, ctx) => new SinusioidalSourceBehavior(def));
            SetParam<ExponentialBehaviorParams>((def, ctx) => new ExponentialSourceBehavior(def));
            SetParam<SffmBehaviorParams>((def, ctx) => new SffmSourceBehavior(def));
            SetParam<AmBehaviorParams>((def, ctx) => new AmSourceBehavior(def));
        }
        
        protected override LargeSignalCircuitModel Instantiate(IModelInstantiationContext<LargeSignalCircuitModel> context)
        {
            var elements = context.CircuitDefinition.Elements.Select(context.GetModel).Cast<ILargeSignalDeviceModel>().ToList();

            return new LargeSignalCircuitModel(context.CircuitDefinition.InitialVoltages, elements);
        }
    }
}