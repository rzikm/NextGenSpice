using System.Composition;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal
{
    [Export(typeof(IAnalysisModelFactory<LargeSignalCircuitModel>))]
    public class LargeSignalModelFactory : ModelFactory<LargeSignalCircuitModel>
    {
        public LargeSignalModelFactory()
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
        }

        private ILargeSignalDeviceModel InstantiateModel(ICircuitDefinitionElement element)
        {
            var model = GetModel(element);
            return (ILargeSignalDeviceModel) model;
        }

        public override LargeSignalCircuitModel Create(ICircuitDefinition circuitDefinition)
        {
            var elements = circuitDefinition.Elements.Select(InstantiateModel).ToList();

            return new LargeSignalCircuitModel(circuitDefinition.InitialVoltages, elements);
        }
    }
}