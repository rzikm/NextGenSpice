namespace NextGenSpice.LargeSignal.Models
{
    public interface IDeviceStatsProvider
    {
        /// <summary>Name of the stat of this provider. That is e.g. "I" for two current throught a two-node device.</summary>
        string StatName { get; }

        /// <summary>Returns value of attribute corresponding to this provider.</summary>
        /// <returns>Value of device parameter corresponding to this provider.</returns>
        double GetValue();
    }
}