﻿using System;
using System.Linq;
using System.Linq.Expressions;
using NextGenSpice.Parser.Statements.Models;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser.Statements.Devices
{
	/// <summary>Base class implementing basic functionality of parsing SPICE .MODEL parameters.</summary>
	/// <typeparam name="T"></typeparam>
	public abstract class DeviceModelHandlerBase<T> : IDeviceModelHandler
	{
		protected DeviceModelHandlerBase()
		{
			Mapper = new ParameterMapper<T>();
		}

		/// <summary>Mapper for mapping parsed parameters onto properties.</summary>
		private ParameterMapper<T> Mapper { get; }

		/// <summary>Discriminator of handled model type.</summary>
		public abstract string Discriminator { get; }

		/// <summary>Processes the .MODEL statement in given context.</summary>
		/// <param name="tokens"></param>
		/// <param name="context"></param>
		public void Process(Token[] tokens, ParsingContext context)
		{
			var name = tokens[1].Value;

			if (context.SymbolTable.TryGetModel<T>(name, out _))
			{
				context.Errors.Add(tokens[1]
					.ToError(SpiceParserErrorCode.ModelAlreadyExists));
				return; // no additional processing required
			}

			Mapper.Target = CreateDefaultModel();
			foreach (var token in tokens.Skip(3)) // skip .MODEL <model name> <discriminator> tokens
			{
				// parameters are must be in key-value pairs <parameter name>=<value> (without whitespace)
				var index = token.Value.IndexOf('=');

				if (index <= 0 || index >= token.Value.Length - 1) // no '=' 
				{
					context.Errors.Add(
						token.ToError(SpiceParserErrorCode.InvalidParameter));
					continue;
				}

				var paramName = token.Value.Substring(0, index);

				// check validity of the parameter name
				if (!Mapper.HasKey(paramName))
					context.Errors.Add(token.ToError(SpiceParserErrorCode.InvalidParameter, paramName));

				// reuse token instance for parsing the value part of the pair
				token.LineColumn += index + 1; // modify offset to get correct error location.
				token.Value = token.Value.Substring(index + 1);

				if (Mapper.HasKey(paramName)) Mapper.Set(paramName, token.GetNumericValue(context.Errors));
			}

			context.SymbolTable.AddModel(Mapper.Target, name);
			Mapper.Target = default(T); // free memory
		}

		/// <summary>Creates new instance of model parameter class with default values.</summary>
		/// <returns></returns>
		object IDeviceModelHandler.CreateDefaultModel()
		{
			return CreateDefaultModel();
		}

		protected void Map(Expression<Func<T, double>> mapping, string paramKey)
		{
			Mapper.Map(mapping, paramKey);
		}

		/// <summary>Creates new instance of parameter class for this device model.</summary>
		/// <returns></returns>
		protected abstract T CreateDefaultModel();
	}
}