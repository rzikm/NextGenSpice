namespace NextGenSpice.Parser
{
    public enum SpiceParserError
    {
        ElementAlreadyDefined,

        NotANumber,
        NotANode,
        NotAnElement,
        NotANodeOrElement,

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
        UnknownElement,

        NoDcPathToGround,
        VoltageBranchCycle,
        CurrentBranchCutset,

        NoPrintProvider,

        NoSuchModel,
        NoSuchSubcircuit,
        InvalidTerminalCount
    }
}