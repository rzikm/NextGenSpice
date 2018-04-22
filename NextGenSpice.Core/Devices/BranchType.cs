namespace NextGenSpice.Core.Devices
{
    /// <summary>Specifies branch type for circuit validation purposes.</summary>
    public enum BranchType
    {
        /// <summary>Branch is defined by foltage across the branch, e.g. voltage source or ideal inductor.</summary>
        VoltageDefined,

        /// <summary>Branch is defined by current flowing through the branch, e.g. current source or ideal capacitor.</summary>
        CurrentDefined,
    }
}