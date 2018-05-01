using System;
using NextGenSpice.Core.Representation;

namespace NextGenSpice.LargeSignal.Devices
{
    /// <summary>Simple implementation of IPrintValueProvider that wraps simple lambdas.</summary>
    internal class SimpleDeviceStateProvider : IDeviceStateProvider
    {
        private readonly Func<double> getter;

        public SimpleDeviceStateProvider(string stat, Func<double> getter)
        {
            this.getter = getter;
            Name = stat;
        }

        /// <summary>Returns value of attribute corresponding to this provider.</summary>
        /// <returns>Value of device parameter corresponding to this provider.</returns>
        public double GetValue()
        {
            return getter();
        }

        /// <summary>Name of the stat of this provider. That is e.g. "I" for two current throught a two-node device.</summary>
        public string Name { get; }
    }
}