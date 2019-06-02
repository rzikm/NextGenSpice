namespace NextGenSpice.Core.Representation
{
    public interface IDeviceStateProvider
    {
        /// <summary>Name of the state variable of this provider. That is e.g. "I" for two current through a two-node device.</summary>
        string Name { get; }

        /// <summary>Returns value of attribute corresponding to this provider.</summary>
        /// <returns>Value of device parameter corresponding to this provider.</returns>
        double GetValue();
    }
}