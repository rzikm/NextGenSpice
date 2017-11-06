using System;
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Extensions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Models;
using Xunit;

namespace NextGenSpiceTest
{
    public class AnalysisModelFactoryTests
    {
        #region Types for testing purposes

        [Export(typeof(IAnalysisModelFactory<TestAnalysisCircuitModel>))]
        class TestAnalysisModelFactory : AnalysisModelFactory<TestAnalysisCircuitModel>
        {
            public override TestAnalysisCircuitModel Create(ICircuitDefinition circuitDefinition)
            {
                return new TestAnalysisCircuitModel(circuitDefinition.Elements.Select(GetModel).Cast<ITestDeviceModel>().ToList());
            }
        }

        class TestAnalysisCircuitModel : IAnalysisCircuitModel<ITestDeviceModel>
        {
            public TestAnalysisCircuitModel(IReadOnlyList<ITestDeviceModel> elements)
            {
                Elements = elements;
            }

            public IReadOnlyList<ITestDeviceModel> Elements { get; }
        }

        class TestDeviceDefinition : TwoNodeCircuitElement
        {

        }

        interface ITestDeviceModel : IAnalysisDeviceModel<TestAnalysisCircuitModel>
        {

        }

        class TestDeviceModel : ITestDeviceModel
        {

        }

        #endregion


        private readonly ICircuitDefinition definition;
        public AnalysisModelFactoryTests()
        {
            definition = CircuitGenerator.GetCircuitWithBasicDevices();
        }

        class MyPrivateFactory : AnalysisModelFactory<LargeSignalCircuitModel>
        {
            public override LargeSignalCircuitModel Create(ICircuitDefinition circuitDefinition)
            {
                return new LargeSignalCircuitModel(new double[5], new List<ILargeSignalDeviceModel>());
            }
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
        public void ThrowsWhenNoFactoryExists()
        {
            Assert.Throws<InvalidOperationException>(() => definition.GetFactory<object>());
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
        public void HasModelsForDefaultTypes()
        {
            definition.GetModel<LargeSignalCircuitModel>();
        }

        [Fact]
        public void CanRegisterNewDevice()
        {
            var circuitDef = new CircuitBuilder().AddElement(new[] { 0, 1 }, new TestDeviceDefinition()).AddElement(new[] { 1, 0 }, new TestDeviceDefinition()).Build();

            circuitDef.SetFactory(new MyPrivateFactory());
            circuitDef.GetFactory<TestAnalysisCircuitModel>().SetModel<TestDeviceDefinition, TestDeviceModel>();
            
            Assert.NotNull(circuitDef.GetModel<TestAnalysisCircuitModel>());
        }

        [Fact]
        public void CanRegisterNewDeviceAsSingleton()
        {
            var circuitDef = new CircuitBuilder().AddElement(new[] { 0, 1 }, new TestDeviceDefinition()).AddElement(new[] { 1, 0 }, new TestDeviceDefinition()).Build();
            circuitDef.SetFactory(new MyPrivateFactory());

            var model = new TestDeviceModel();
            circuitDef.GetFactory<TestAnalysisCircuitModel>().SetModel<TestDeviceDefinition, TestDeviceModel>(def => model);

            var circuitModel = circuitDef.GetModel<TestAnalysisCircuitModel>();
            Assert.NotNull(circuitModel);
            Assert.Equal(circuitModel.Elements[0], circuitModel.Elements[1]);
        }

        [Fact]
        public void ThrowsWhenNoModelCreatorExists()
        {
            var circuitDef = new CircuitBuilder().AddElement(new[] { 0, 1 }, new TestDeviceDefinition()).AddElement(new[] { 1, 0 }, new TestDeviceDefinition()).Build();
            Assert.Throws<InvalidOperationException>(() => circuitDef.GetModel<LargeSignalCircuitModel>());
        }

    }
}