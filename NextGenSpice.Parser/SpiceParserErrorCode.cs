namespace NextGenSpice.Parser
{
    public enum SpiceParserErrorCode
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
        InvalidParameter,

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
        InvalidTerminalCount,

        UnexpectedEnds,
        NotVoltageSource
    }
}