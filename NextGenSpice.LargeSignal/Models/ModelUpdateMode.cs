namespace NextGenSpice.LargeSignal.Models
{
    /// <summary>Specifies how often a device model should be updated.</summary>
    public enum ModelUpdateMode
    {
        /// <summary>
        ///     A stamp is taken on the beginning of the simulation and is not updated since. Suitable for linear devices that
        ///     do not change their models over time (e.g. resistor).
        /// </summary>
        NoUpdate,

        /// <summary>
        ///     Model is stamped once at the beginning of DC Bias calculation for each timepoint. Suitable for linear models
        ///     that are time-dependent (e.g. capacitor).
        /// </summary>
        TimePoint,

        /// <summary>Model is stamped every iteration of Newton-Raphson algorithm. Suitable for nonlinear models (e.g. diode).</summary>
        Always
    }
}