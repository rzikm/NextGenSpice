﻿using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Devices;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Deferring
{
	/// <summary>
	///   Class representing a deferred statement for voltage source dependent statements.
	/// </summary>
	public class VoltageSourceDependentDeferredStatement : SimpleDeviceDeferredStatement
	{
		private readonly Token vsourcName;

		public VoltageSourceDependentDeferredStatement(ParsingScope scope, Token vsourcName,
			Action<CircuitBuilder, VoltageSource> builderFunc)
			: base(scope, b => builderFunc(b, FindVoltageSource(b, vsourcName.Value)))
		{
			this.vsourcName = vsourcName;
		}

		/// <summary>Returns true if all prerequisites for the statements have been fulfilled and statement is ready to be applied.</summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool CanApply()
		{
			var vs = FindVoltageSource(Scope.CircuitBuilder, vsourcName.Value);
			return vs != null;
		}

		private static VoltageSource FindVoltageSource(CircuitBuilder builder, string name)
		{
			return builder.Devices.FirstOrDefault(dev => Equals(dev.Tag, name)) as VoltageSource;
		}

		/// <summary>Calling this function always results in InvalidOperationException as this statement can always be processed.</summary>
		/// <returns></returns>
		public override IEnumerable<SpiceParserError> GetErrors()
		{
			return new[] {vsourcName.ToError(SpiceParserErrorCode.NotVoltageSource, vsourcName.Value)};
		}
	}
}