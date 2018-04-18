﻿using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Representation
{
    /// <summary>Context in which a certain circuit definition is instantiated</summary>
    /// <typeparam name="TAnalysisModel"></typeparam>
    public class ModelInstantiationContext<TAnalysisModel> : IModelInstantiationContext<TAnalysisModel>
    {
        private readonly Dictionary<Type, Func<ICircuitDefinitionDevice, IModelInstantiationContext<TAnalysisModel>,
            IAnalysisDeviceModel<TAnalysisModel>>> modelCreators;

        private readonly Dictionary<string, ICircuitDefinitionDevice> namedDevices;

        private readonly Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>>
            paramCreators;

        private readonly Dictionary<ICircuitDefinitionDevice, IAnalysisDeviceModel<TAnalysisModel>>
            resolutionCache; // cached models so we can compose arbitrary object graph.

        public ModelInstantiationContext(
            Dictionary<Type, Func<ICircuitDefinitionDevice, IModelInstantiationContext<TAnalysisModel>,
                IAnalysisDeviceModel<TAnalysisModel>>> modelCreators,
            Dictionary<Type, Func<object, IModelInstantiationContext<TAnalysisModel>, object>> paramCreators,
            ICircuitDefinition circuitDefinition)
        {
            this.paramCreators = paramCreators;
            this.modelCreators = modelCreators;
            CircuitDefinition = circuitDefinition;
            resolutionCache = new Dictionary<ICircuitDefinitionDevice, IAnalysisDeviceModel<TAnalysisModel>>();
            namedDevices = circuitDefinition.Devices.Where(e => e.Name != null).ToDictionary(e => e.Name);
        }

        /// <summary>Current circuit definition.</summary>
        public ICircuitDefinition CircuitDefinition { get; }

        /// <summary>Gets model instance for a given device definition instance.</summary>
        /// <param name="device">The device definition.</param>
        /// <returns></returns>
        public IAnalysisDeviceModel<TAnalysisModel> GetModel(ICircuitDefinitionDevice device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));

            if (resolutionCache.TryGetValue(device, out var model))
                return model;

            return resolutionCache[device] = CreateModel(device);
        }

        /// <summary>Creates model instance for definition device with given name.</summary>
        /// <param name="deviceName">Name of the device to be instantiated.</param>
        /// <returns></returns>
        public IAnalysisDeviceModel<TAnalysisModel> GetModel(string deviceName)
        {
            if (deviceName == null) throw new ArgumentNullException(nameof(deviceName));

            if (!namedDevices.TryGetValue(deviceName, out var device))
                throw new ArgumentException($"Device with name {deviceName} does not exist in given context.");

            return GetModel(device);
        }

        /// <summary>Processes parameter using registered factory function for its type.</summary>
        /// <param name="arg">Argument to be processed.</param>
        /// <returns></returns>
        public object GetParam(object arg)
        {
            if (paramCreators.TryGetValue(arg.GetType(), out var factoryFunc))
                return factoryFunc(arg, this);

            throw new InvalidOperationException($"No param creator for type{arg.GetType()}");
        }

        /// <summary>Creates model instance for given definition device.</summary>
        /// <param name="device">Device to be instantiated.</param>
        /// <returns></returns>
        private IAnalysisDeviceModel<TAnalysisModel> CreateModel(ICircuitDefinitionDevice device)
        {
            if (modelCreators.TryGetValue(device.GetType(), out var factory))
                return factory(device, this);

            throw new InvalidOperationException($"No model creator for type {device.GetType()}");
        }
    }
}