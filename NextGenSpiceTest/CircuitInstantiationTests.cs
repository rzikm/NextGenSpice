using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using Xunit;

namespace NextGenSpiceTest
{
    public class CircuitInstantiationTests
    {
        public CircuitInstantiationTests()
        {
            circuitDefinition = CircuitGenerator.GetLinearCircuit();
            var modelCreators =
                new Dictionary<Type, Func<ICircuitDefinitionDevice, IModelInstantiationContext<LargeSignalCircuitModel>
                    , IAnalysisDeviceModel<LargeSignalCircuitModel>>>
                {
                    [typeof(ResistorDevice)] =
                        (e, ctx) => new LargeSignalResistor((ResistorDevice) e),
                    [typeof(VoltageSourceDevice)] =
                        (e, ctx) => new LargeSignalVoltageSource((VoltageSourceDevice) e, null)
                };


            context = new ModelInstantiationContext<LargeSignalCircuitModel>(modelCreators,
                new Dictionary<Type, Func<object, IModelInstantiationContext<LargeSignalCircuitModel>, object>>(),
                circuitDefinition);
        }

        private readonly ModelInstantiationContext<LargeSignalCircuitModel> context;
        private readonly CircuitDefinition circuitDefinition;

        [Fact]
        public void GetsCachedModel()
        {
            var device = circuitDefinition.Devices.First();

            var model = context.GetModel(device);

            Assert.Equal(model, context.GetModel(device));
        }

        [Fact]
        public void GetsModelByName()
        {
            var modelName = "R1";

            var device = circuitDefinition.Devices.Single(e => e.Tag == modelName);
            var model = context.GetModel(modelName);

            Assert.Equal(context.GetModel(device), model);
        }

        [Fact]
        public void ThrowWhenNoSuchNameExists()
        {
            Assert.Throws<ArgumentNullException>(() => context.GetModel((string) null));
            Assert.Throws<ArgumentException>(() => context.GetModel("nonexistant model"));

            Assert.Throws<ArgumentNullException>(() => context.GetModel((ICircuitDefinitionDevice) null));
        }
    }
}