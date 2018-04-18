namespace NextGenSpice.Core.Parser
{
    public enum SpiceParserError
    {
        DeviceAlreadyDefined,

        NotANumber,
        NotANode,
        NotAnDevice,
        NotANodeOrDevice,

        InvalidNumberOfArguments,
        UnexpectedCharacter,
        UnknownTransientFunction,

        NoBreakpointRepetition,
        TimePointWithoutValue,
        NegativeTimepoint,
        NonascendingTimepoints,

        ModelAlreadyExists,
        InvalidModelParameter,
        UnknownParameter,

        InvalidIcArgument,

        UnknownDeviceModelDiscriminator,

        UnknownPrintStatementParameter,

        UnknownAnalysisType,

        SubcircuitAlreadyExists,

        TerminalNamesNotUnique,
        TerminalToGround,

        SubcircuitNotConnected,

        UnknownStatement,
        UnknownDevice,

        NoDcPathToGround,
        VoltageBranchCycle,
        CurrentBranchCutset,

        NoPrintProvider,

        NoSuchModel,
        NoSuchSubcircuit,
        InvalidTerminalCount
    }
}