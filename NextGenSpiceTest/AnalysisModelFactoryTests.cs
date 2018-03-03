using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using Xunit;

namespace NextGenSpiceTest
{
    public class AnalysisModelFactoryTests
    {
        public AnalysisModelFactoryTests()
        {
            definition = CircuitGenerator.GetCircuitWithBasicDevices();
        }

        [Export(typeof(IAnalysisModelFactory<TestAnalysisCircuitModel>))]
        private class TestAnalysisModelFactory : AnalysisModelFactory<TestAnalysisCircuitModel>
        {
            protected override TestAnalysisCircuitModel Instantiate(
                IModelInstantiationContext<TestAnalysisCircuitModel> context)
            {
                return new TestAnalysisCircuitModel(context.CircuitDefinition.Elements.Select(context.GetModel)
                    .Cast<ITestDeviceModel>().ToList());
            }
        }

        private class TestAnalysisCircuitModel : IAnalysisCircuitModel<ITestDeviceModel>
        {
            public TestAnalysisCircuitModel(IReadOnlyList<ITestDeviceModel> elements)
            {
                Elements = elements;
            }

            public IReadOnlyList<ITestDeviceModel> Elements { get; }
        }

        private class TestDeviceDefinition : TwoNodeCircuitElement
        {
            public TestDeviceDefinition(string name = null) : base(name)
            {
            }

            /// <summary>
            ///     Gets metadata about this device interconnections in the circuit.
            /// </summary>
            /// <returns></returns>
            public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
            {
                return new []
                {
                    new CircuitBranchMetadata(Anode, Cathode, BranchType.Mixed, this)
                };
            }
        }

        private interface ITestDeviceModel : IAnalysisDeviceModel<TestAnalysisCircuitModel>
        {
        }

        private class TestDeviceModel : ITestDeviceModel
        {
            /// <summary>
            ///     Instance of definition element that corresponds to this device analysis model.
            /// </summary>
            public ICircuitDefinitionElement DefinitionElement => throw new NotImplementedException();
        }


        private readonly ICircuitDefinition definition;

        private class MyPrivateFactory : AnalysisModelFactory<LargeSignalCircuitModel>
        {
            protected override LargeSignalCircuitModel Instantiate(
                IModelInstantiationContext<LargeSignalCircuitModel> context)
            {
                return new LargeSignalCircuitModel(new double?[5], new List<ILargeSignalDeviceModel>());
            }
        }

        [Fact]
        public void CanRegisterNewDeviceModel()
        {
            var circuitDef = new CircuitBuilder().AddElement(new[] {0, 1}, new TestDeviceDefinition())
                .AddElement(new[] {1, 0}, new TestDeviceDefinition()).BuildCircuit();

            circuitDef.SetFactory(new MyPrivateFactory());
            circuitDef.GetFactory<TestAnalysisCircuitModel>()
                .SetModel<TestDeviceDefinition, TestDeviceModel>(def => new TestDeviceModel());

            Assert.NotNull(circuitDef.GetModel<TestAnalysisCircuitModel>());
        }

        [Fact]
        public void FindsMefExportedFactory()
        {
            Assert.NotNull(definition.GetFactory<LargeSignalCircuitModel>());
        }

        [Fact]
        public void FindsMefExportedFactoryFromThisAssembly()
        {
            Assert.NotNull(definition.GetFactory<TestAnalysisCircuitModel>());
        }

        [Fact]
        public void HasModelsForDefaultDevices()
        {
            definition.GetModel<LargeSignalCircuitModel>();
        }

        [Fact]
        public void ManuallyRegisteredFactoryHasPrecedence()
        {
            definition.SetFactory(new MyPrivateFactory());

            var factory = definition.GetFactory<LargeSignalCircuitModel>();

            Assert.Equal(typeof(MyPrivateFactory), factory.GetType());
            definition.GetModel<LargeSignalCircuitModel>();
        }

        [Fact]
        public void ThrowsWhenNoFactoryExists()
        {
            Assert.Throws<InvalidOperationException>(() => definition.GetFactory<object>());
        }

        [Fact]
        public void ThrowsWhenNoModelCreatorExists()
        {
            var circuitDef = new CircuitBuilder().AddElement(new[] {0, 1}, new TestDeviceDefinition())
                .AddElement(new[] {1, 0}, new TestDeviceDefinition()).BuildCircuit();
            Assert.Throws<InvalidOperationException>(() => circuitDef.GetModel<LargeSignalCircuitModel>());
        }
    }
}