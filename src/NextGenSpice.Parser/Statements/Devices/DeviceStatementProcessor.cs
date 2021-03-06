﻿using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Devices
{
	/// <summary>Class representing processor for a given device type.</summary>
	public abstract class DeviceStatementProcessor : IDeviceStatementProcessor
	{
		private int oldErrors;
		private ISymbolTable SymbolTable => Context.SymbolTable;

		/// <summary>Number of errors that occured during parsing of this statement.</summary>
		protected int Errors => Context.Errors.Count - oldErrors;

		/// <summary>Circuit builder for the current parsing context.</summary>
		protected CircuitBuilder CircuitBuilder => Context.CurrentScope.CircuitBuilder;

		/// <summary>Current context in which parsing occurs.</summary>
		protected ParsingContext Context { get; private set; }

		/// <summary>Maximum number of arguments (excluding name) for the processed statement.</summary>
		protected int MinArgs { get; set; } = 0;

		/// <summary>Minimum number of arguments (excluding name) for the processed statement.</summary>
		protected int MaxArgs { get; set; } = int.MaxValue;

		/// <summary>Parsed name from the first token.</summary>
		protected string DeviceName => RawStatement[0]?.Value;

		/// <summary>Unprocessed tokens that make up the device statement.</summary>
		protected Token[] RawStatement { get; private set; }

		/// <summary>Discriminator of the device type this processor can parse.</summary>
		public abstract char Discriminator { get; }

		/// <summary>Parses given line of tokens, adds statement to be processed later or adds errors to Errors collection.</summary>
		/// <param name="tokens"></param>
		/// <param name="ctx"></param>
		/// <returns></returns>
		public void Process(Token[] tokens, ParsingContext ctx)
		{
			// set context for the derived classes
			Context = ctx;
			RawStatement = tokens;

			oldErrors = ctx.Errors.Count;

			if (tokens.Length - 1 < MinArgs || tokens.Length - 1 > MaxArgs)
				InvalidNumberOfArguments(tokens[0]); // there is always at least one token.

			DeclareDevice(tokens[0]);

			DoProcess();

			Context = null;
			RawStatement = null;
		}

		/// <summary>Gets list of model statement handlers that are responsible to parsing respective models of this device.</summary>
		/// <returns></returns>
		public virtual IEnumerable<IDeviceModelHandler> GetModelStatementHandlers()
		{
			return Enumerable.Empty<IDeviceModelHandler>();
		}

		/// <summary>Processes given set of statements.</summary>
		protected abstract void DoProcess();

		/// <summary>Returns message, that some device with given name has been already defined.</summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private void DeviceAlreadyDefined(Token token)
		{
			Context.Errors.Add(token.ToError(SpiceParserErrorCode.DeviceAlreadyDefined));
		}

		/// <summary>Return message, that given token cannot be converted to a numeric representation.</summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private void NotANumber(Token token)
		{
			Context.Errors.Add(token.ToError(SpiceParserErrorCode.NotANumber));
		}

		/// <summary>Returns message indicating that given token does not represent a node name.</summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private void NotANode(Token token)
		{
			Context.Errors.Add(token.ToError(SpiceParserErrorCode.NotANode));
		}

		/// <summary>Return message indicatiing that there was wrong number of arguments for given device type.</summary>
		/// <param name="token"></param>
		/// <returns></returns>
		protected void InvalidNumberOfArguments(Token token)
		{
			Context.Errors.Add(token.ToError(SpiceParserErrorCode.InvalidNumberOfArguments));
		}

		/// <summary>Gets device name and sets it in symbol table, adds relevant errors into the errors collection</summary>
		/// <param name="token"></param>
		/// <returns></returns>
		private void DeclareDevice(Token token)
		{
			var name = token.Value;
			if (!SymbolTable.TryDefineDevice(name)) DeviceAlreadyDefined(token);
		}

		/// <summary>
		///   Gets indices of the nodes represented by tokens starting at startIndex. Adds relevant errors into the errors
		///   collection
		/// </summary>
		/// <param name="tokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		private int[] GetNodeIds(Token[] tokens, int startIndex, int count)
		{
			var ret = new int[count];
			for (var i = 0; i < Math.Min(tokens.Length - startIndex, count); i++)
			{
				var token = tokens[startIndex + i];
				if (!SymbolTable.TryGetOrCreateNode(token.Value, out var node))
				{
					node = -1;
					NotANode(token);
				}

				ret[i] = node;
			}

			return ret;
		}

		/// <summary>
		///   Gets indices of the nodes represented by tokens starting at startIndex. Adds relevant errors into the errors
		///   collection
		/// </summary>
		/// <param name="tokens"></param>
		/// <param name="startIndex"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		protected int[] GetNodeIds(int startIndex, int count)
		{
			return GetNodeIds(RawStatement, startIndex, count);
		}

		/// <summary>
		///   Parses numeric value from given token, adds relevant error into the errors collection and returns NaN if
		///   failed.
		/// </summary>
		/// <param name="index">Index of the token in the statement.</param>
		/// <returns></returns>
		protected double GetValue(int index)
		{
			if (index < 0 || index >= RawStatement.Length) throw new IndexOutOfRangeException();
			return GetValue(RawStatement[index]);
		}

		/// <summary>
		///   Parses numeric value from given token, adds relevant error into the errors collection and returns NaN if
		///   failed.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		protected double GetValue(Token token)
		{
			var value = Utils.Parser.ConvertValue(token.Value);
			if (double.IsNaN(value)) NotANumber(token);
			return value;
		}
	}
}