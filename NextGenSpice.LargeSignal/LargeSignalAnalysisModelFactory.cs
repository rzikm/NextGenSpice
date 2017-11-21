using System.Composition;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Representation;
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
            SetModel<CurrentSourceElement, LargeSignalCurrentSourceModel>(e => new LargeSignalCurrentSourceModel(e));
            SetModel<VoltageSourceElement, LargeSignalVoltageSourceModel>(e => new LargeSignalVoltageSourceModel(e));
            SetModel<CapacitorElement, LargeSignalCapacitorModel>(e => new LargeSignalCapacitorModel(e));
            SetModel<InductorElement, LargeSignalInductorModel>(e => new LargeSignalInductorModel(e));
            SetModel<SubcircuitElement, LargeSignalSubcircuitModel>((e, f) => new LargeSignalSubcircuitModel(e, e.Elements.Select(f.GetModel).Cast<ILargeSignalDeviceModel>()));
        }

        private ILargeSignalDeviceModel InstantiateModel(ICircuitDefinitionElement element)
        {
            var model = GetModel(element);
            return (ILargeSignalDeviceModel) model;
        }

        protected override LargeSignalCircuitModel Instantiate(ModelInstantiationContext<LargeSignalCircuitModel> context)
        {
            var elements = context.CircuitDefinition.Elements.Select(context.GetModel).Cast<ILargeSignalDeviceModel>().ToList();

            return new LargeSignalCircuitModel(context.CircuitDefinition.InitialVoltages, elements);
        }
    }
}