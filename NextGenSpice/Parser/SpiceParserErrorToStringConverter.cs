using System;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice.Parser
{
    public static class SpiceParserErrorToStringConverter
    {
        public static string GetMessage(this SpiceParserError errorCode, params object[] arg)
        {
            if (arg == null) throw new ArgumentNullException(nameof(arg));

            switch (errorCode)
            {
                case SpiceParserError.ElementAlreadyDefined:
                    return $"Element with name {arg[0]} is already defined.";

                case SpiceParserError.NotANumber:
                    return $"Cannot convert {arg[0]} to numeric representation.";

                case SpiceParserError.NotANode:
                    return $"Symbol {arg[0]} is not a node";

                case SpiceParserError.InvalidNumberOfArguments:
                    return $"Invalid number of arguments for '{arg[0]}'";

                case SpiceParserError.UnexpectedCharacter:
                    return $"Unexpected character: '{arg[0]}'.";

                case SpiceParserError.UnknownTransientFunction:
                    return $"Unknown transient source function: '{arg[0]}'";

                case SpiceParserError.NoBreakpointRepetition:
                    return "Repetition point must be equal to a function breakpoint.";

                case SpiceParserError.TimePointWithoutValue:
                    return "Timpeoint without corresponding value.";

                case SpiceParserError.NegativeTimepoint:
                    return "Timepoints must be nonnegative.";

                case SpiceParserError.NonascendingTimepoints:
                    return "Timepoints must be ascending.";

                case SpiceParserError.ModelAlreadyExists:
                    return $"There already exists model with name '{arg[0]} for this device type.";

                case SpiceParserError.InvalidModelParameter:
                    return $"Invalid model parameter '{arg[0]}'";

                case SpiceParserError.UnknownParameter:
                    return $"Unknown model parameter name '{arg[0]}'.";

                case SpiceParserError.InvalidIcArgument:
                    return $"Invalid .IC argument '{arg[0]}'";

                case SpiceParserError.UnknownDeviceModelDiscriminator:
                    return $"Unknown device model discriminator'{arg[0]}'";

                case SpiceParserError.UnknownPrintStatementParameter:
                    return $"Unknown .PRINT statement parameter '{arg[0]}'.";

                case SpiceParserError.UnknownAnalysisType:
                    return $"Unknown analysis type: '{arg[0]}'.";

                case SpiceParserError.SubcircuitAlreadyExists:
                    return $"Subcircuit with name '{arg[0]}' already exists.";

                case SpiceParserError.TerminalNamesNotUnique:
                    return "Terminal names must be unique.";

                case SpiceParserError.TerminalToGround:
                    return "Cannot specify ground node as a subcircuit terminal.";

                case SpiceParserError.SubcircuitNotConnected:
                    return
                        $"No path connecting node sets {string.Join(", ", arg.Select(c => $"({string.Join(", ", c as IEnumerable<object>)})"))}.";

                case SpiceParserError.UnknownStatement:
                    return $"Unknown statement '{arg[0]}'";

                case SpiceParserError.UnknownElement:
                    return $"Unknown element type '{arg[0]}'";

                case SpiceParserError.NoDcPathToGround:
                    return $"Some nodes are not connected to the ground node ({string.Join(", ", arg)}).";

                case SpiceParserError.VoltageBranchCycle:
                    return $"Circuit contains a cycle of voltage defined elements ({string.Join(", ", arg)}).";

                case SpiceParserError.CurrentBranchCutset:
                    return $"Circuit contains a cutset of current defined elements ({string.Join(", ", arg)}).";

                case SpiceParserError.NoPrintProvider:
                    return $"There is no print value provider for '{arg[0]}' for device '{arg[1]}'.";

                case SpiceParserError.NotAnElement:
                    return $"'{arg[0]}' is not a circuit element.";

                case SpiceParserError.NotANodeOrElement:
                    return $"'{arg[0]}' is not a node or a circuit element.";

                case SpiceParserError.NoSuchModel:
                    return $"There is no model named '{arg[0]}' for this device type.";

                case SpiceParserError.NoSuchSubcircuit:
                    return $"There is no model named '{arg[0]}' for this device type.";

                case SpiceParserError.InvalidTerminalCount:
                    return $"Subcircuit has wrong number of terminals '{arg[0]}'";

                default:
                    throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null);
            }
        }
    }
}