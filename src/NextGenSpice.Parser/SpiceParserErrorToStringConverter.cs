using System;
using System.Collections.Generic;
using System.Linq;

namespace NextGenSpice.Parser
{
	public static class SpiceParserErrorToStringConverter
	{
		public static string GetMessage(this SpiceParserErrorCode errorCode, params object[] arg)
		{
			if (arg == null) throw new ArgumentNullException(nameof(arg));

			switch (errorCode)
			{
				case SpiceParserErrorCode.DeviceAlreadyDefined:
					return $"Device with name {arg[0]} is already defined.";

				case SpiceParserErrorCode.NotANumber:
					return $"Cannot convert {arg[0]} to numeric representation.";

				case SpiceParserErrorCode.NotANode:
					return $"Symbol {arg[0]} is not a node";

				case SpiceParserErrorCode.InvalidNumberOfArguments:
					return $"Invalid number of arguments for '{arg[0]}'";

				case SpiceParserErrorCode.UnexpectedCharacter:
					return $"Unexpected character: '{arg[0]}'.";

				case SpiceParserErrorCode.UnknownTransientFunction:
					return $"Unknown transient source function: '{arg[0]}'";

				case SpiceParserErrorCode.NoBreakpointRepetition:
					return "Repetition point must be equal to a function breakpoint.";

				case SpiceParserErrorCode.TimePointWithoutValue:
					return "Timpeoint without corresponding value.";

				case SpiceParserErrorCode.NegativeTimepoint:
					return "Timepoints must be nonnegative.";

				case SpiceParserErrorCode.NonascendingTimepoints:
					return "Timepoints must be ascending.";

				case SpiceParserErrorCode.ModelAlreadyExists:
					return $"There already exists model with name '{arg[0]} for this device type.";

				case SpiceParserErrorCode.InvalidParameter:
					return $"Invalid parameter '{arg[0]}'.";

				case SpiceParserErrorCode.InvalidIcArgument:
					return $"Invalid .IC argument '{arg[0]}'";

				case SpiceParserErrorCode.UnknownDeviceModelDiscriminator:
					return $"Unknown device model discriminator'{arg[0]}'";

				case SpiceParserErrorCode.UnknownPrintStatementParameter:
					return $"Unknown .PRINT statement parameter '{arg[0]}'.";

				case SpiceParserErrorCode.UnknownAnalysisType:
					return $"Unknown analysis type: '{arg[0]}'.";

				case SpiceParserErrorCode.SubcircuitAlreadyExists:
					return $"Subcircuit with name '{arg[0]}' already exists.";

				case SpiceParserErrorCode.TerminalNamesNotUnique:
					return "Terminal names must be unique.";

				case SpiceParserErrorCode.TerminalToGround:
					return "Cannot specify ground node as a subcircuit terminal.";

				case SpiceParserErrorCode.SubcircuitNotConnected:
					return
						$"No path connecting node sets {string.Join(", ", arg.Select(c => $"({string.Join(", ", c as IEnumerable<object>)})"))}.";

				case SpiceParserErrorCode.UnknownStatement:
					return $"Unknown statement '{arg[0]}'";

				case SpiceParserErrorCode.UnknownDevice:
					return $"Unknown device type '{arg[0]}'";

				case SpiceParserErrorCode.NoDcPathToGround:
					return $"Some nodes are not connected to the ground node ({string.Join(", ", arg)}).";

				case SpiceParserErrorCode.VoltageBranchCycle:
					return $"Circuit contains a cycle of voltage defined devices ({string.Join(", ", arg)}).";

				case SpiceParserErrorCode.CurrentBranchCutset:
					return $"Circuit contains a cutset of current defined devices ({string.Join(", ", arg)}).";

				case SpiceParserErrorCode.NoPrintProvider:
					return $"There is no print value provider for '{arg[0]}' for device '{arg[1]}'.";

				case SpiceParserErrorCode.NotAnDevice:
					return $"'{arg[0]}' is not a circuit device.";

				case SpiceParserErrorCode.NotANodeOrDevice:
					return $"'{arg[0]}' is not a node or a circuit device.";

				case SpiceParserErrorCode.NoSuchModel:
					return $"There is no model named '{arg[0]}' for this device type.";

				case SpiceParserErrorCode.NoSuchSubcircuit:
					return $"There is no model named '{arg[0]}' for this device type.";

				case SpiceParserErrorCode.InvalidTerminalCount:
					return $"Subcircuit has wrong number of terminals '{arg[0]}'";

				case SpiceParserErrorCode.UnexpectedEnds:
					return $"Unexpected .ENDS statement for '{arg[0]}'";

				case SpiceParserErrorCode.NotVoltageSource:
					return $"'{arg[0]} is not a Voltage source device.'";
				default:
					throw new ArgumentOutOfRangeException(nameof(errorCode), errorCode, null);
			}
		}
	}
}