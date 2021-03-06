﻿using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal;
using NextGenSpice.LargeSignal.Devices;
using Xunit;

namespace NextGenSpice.Core.Test
{
	public class AnalysisModelFactoryTests
	{
		public AnalysisModelFactoryTests()
		{
			definition = CircuitGenerator.GetCircuitWithBasicDevices();
			creator = new AnalysisModelCreator();
		}

		[Export(typeof(IAnalysisModelFactory<TestAnalysisCircuitModel>))]
		private class TestAnalysisModelFactory : AnalysisModelFactory<TestAnalysisCircuitModel>
		{
			protected override TestAnalysisCircuitModel NewInstance(
				IModelInstantiationContext<TestAnalysisCircuitModel> context)
			{
				return new TestAnalysisCircuitModel(context.CircuitDefinition.Devices.Select(context.GetModel)
					.Cast<ITestDeviceModel>().ToList());
			}
		}

		private class TestAnalysisCircuitModel : IAnalysisCircuitModel<ITestDeviceModel>
		{
			public TestAnalysisCircuitModel(IReadOnlyList<ITestDeviceModel> devices)
			{
				Devices = devices;
			}

			public IReadOnlyList<ITestDeviceModel> Devices { get; }

			public ITestDeviceModel FindDevice(object tag)
			{
				throw new NotImplementedException();
			}

			public ITestDeviceModel FindDevice(ICircuitDefinitionDevice device)
			{
				throw new NotImplementedException();
			}
		}

		private class TestDeviceDefinition : TwoTerminalCircuitDevice
		{
			public TestDeviceDefinition(string name = null) : base(name)
			{
			}

			/// <summary>Gets metadata about this device interconnections in the circuit.</summary>
			/// <returns></returns>
			public override IEnumerable<CircuitBranchMetadata> GetBranchMetadata()
			{
				return Enumerable.Empty<CircuitBranchMetadata>();
			}
		}

		private interface ITestDeviceModel : IAnalysisDeviceModel<TestAnalysisCircuitModel>
		{
		}

		private class TestDeviceModel : ITestDeviceModel
		{
			/// <summary>Instance of definition device that corresponds to this device analysis model.</summary>
			public ICircuitDefinitionDevice DefinitionDevice => throw new NotImplementedException();

			/// <summary>Gets stats provider instances for this device.</summary>
			/// <returns>IPrintValueProviders for specified attribute.</returns>
			public IEnumerable<IDeviceStateProvider> GetDeviceStateProviders()
			{
				return new List<IDeviceStateProvider>();
			}
		}


		private readonly ICircuitDefinition definition;
		private readonly IAnalysisModelCreator creator;

		private class MyPrivateFactory : AnalysisModelFactory<LargeSignalCircuitModel>
		{
			protected override LargeSignalCircuitModel NewInstance(
				IModelInstantiationContext<LargeSignalCircuitModel> context)
			{
				return new LargeSignalCircuitModel(new double?[5], new List<ILargeSignalDevice>());
			}
		}

		[Fact]
		public void AnalysisModelCreatorsAreIndependent()
		{
			var other = new AnalysisModelCreator();
			var f1 = other.GetFactory<TestAnalysisCircuitModel>();
			var f2 = creator.GetFactory<TestAnalysisCircuitModel>();

			Assert.NotEqual(f1, f2);
		}

		[Fact]
		public void CanRegisterNewDeviceModel()
		{
			var circuitDef = new CircuitBuilder().AddDevice(new[] {0, 1}, new TestDeviceDefinition())
				.AddDevice(new[] {1, 0}, new TestDeviceDefinition()).BuildCircuit();

			creator.SetFactory(new MyPrivateFactory());
			creator.GetFactory<TestAnalysisCircuitModel>()
				.SetModel<TestDeviceDefinition, TestDeviceModel>(def => new TestDeviceModel());

			Assert.NotNull(creator.Create<TestAnalysisCircuitModel>(circuitDef));
		}

		[Fact]
		public void FindsMefExportedFactory()
		{
			Assert.NotNull(creator.GetFactory<LargeSignalCircuitModel>());
		}

		[Fact]
		public void FindsMefExportedFactoryFromThisAssembly()
		{
			Assert.NotNull(creator.GetFactory<TestAnalysisCircuitModel>());
		}

		[Fact]
		public void HasModelsForDefaultDevices()
		{
			creator.Create<LargeSignalCircuitModel>(definition);
		}

		[Fact]
		public void ManuallyRegisteredFactoryHasPrecedence()
		{
			creator.SetFactory(new MyPrivateFactory());

			var factory = creator.GetFactory<LargeSignalCircuitModel>();

			Assert.Equal(typeof(MyPrivateFactory), factory.GetType());
			creator.Create<LargeSignalCircuitModel>(definition);
		}

		[Fact]
		public void ThrowsWhenNoFactoryExists()
		{
			Assert.Throws<InvalidOperationException>(() => creator.GetFactory<object>());
		}

		[Fact]
		public void ThrowsWhenNoModelCreatorExists()
		{
			var circuitDef = new CircuitBuilder().AddDevice(new[] {0, 1}, new TestDeviceDefinition())
				.AddDevice(new[] {1, 0}, new TestDeviceDefinition()).BuildCircuit();
			Assert.Throws<InvalidOperationException>(() => creator.Create<LargeSignalCircuitModel>(circuitDef));
		}
	}
}