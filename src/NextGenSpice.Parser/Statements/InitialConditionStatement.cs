﻿using System.Linq;
using System.Text.RegularExpressions;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements
{
	/// <summary>Class for handling statements that specify initial node voltages.</summary>
	public class InitialConditionStatement : DotStatementProcessor
	{
		private readonly Regex argRegex;

		public InitialConditionStatement()
		{
			argRegex = new Regex(@"[vV]\(([^)]+)\)=(.*)", RegexOptions.Compiled);
			MinArgs = 1;
		}

		/// <summary>Statement discriminator, that this class can handle.</summary>
		public override string Discriminator => ".IC";

		/// <summary>Processes given statement.</summary>
		/// <param name="tokens">All tokens of the statement.</param>
		protected override void DoProcess(Token[] tokens)
		{
			foreach (var token in tokens.Skip(1)) // skip ".IC" token
			{
				var s = token.Value;

				var matches = argRegex.Match(s);

				if (!matches.Success)
				{
					Context.Errors.Add(token.ToError(SpiceParserErrorCode.InvalidIcArgument));
					continue;
				}

				var nodeName = matches.Groups[1].Value;
				if (!Context.SymbolTable.TryGetOrCreateNode(nodeName, out var nodeIndex))
				{
					Context.Errors.Add(token.ToError(SpiceParserErrorCode.NotANode));
					nodeIndex = -1;
					continue;
				}

				var value = new Token
				{
					LineNumber = token.LineNumber,
					LineColumn = token.LineColumn + matches.Groups[1].Index,
					Value = matches.Groups[2].Value
				}.GetNumericValue(Context.Errors);

				if (nodeIndex >= 0) Context.CurrentScope.CircuitBuilder.SetNodeVoltage(nodeIndex, value);
			}
		}
	}
}