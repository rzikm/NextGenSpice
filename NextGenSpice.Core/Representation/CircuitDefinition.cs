using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using NextGenSpice.Core.Devices;

namespace NextGenSpice.Core.Representation
{
    /// <summary>Class that represents definition of an electric circuit.</summary>
    public class CircuitDefinition : ICircuitDefinition
    {
       
        public CircuitDefinition(IReadOnlyList<double?> initialVoltages,
            IReadOnlyList<ICircuitDefinitionDevice> devices)
        {
            InitialVoltages = initialVoltages;
            Devices = devices;
        }

        /// <summary>Number of the nodes in the circuit.</summary>
        public int NodeCount => InitialVoltages.Count;

        /// <summary>Initial voltages of nodes by their id.</summary>
        public IReadOnlyList<double?> InitialVoltages { get; }

        /// <summary>Set of devices that define this circuit.</summary>
        public IReadOnlyList<ICircuitDefinitionDevice> Devices { get; }

        
    }
}