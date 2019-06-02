using System;
using System.Runtime.Serialization;

namespace NextGenSpice.Numerics
{
	[Serializable]
	public class ArgumentNaNException : ArgumentException
	{
		public ArgumentNaNException()
		{
		}

		public ArgumentNaNException(string message) : base(message)
		{
		}

		public ArgumentNaNException(string message, Exception inner) : base(message, inner)
		{
		}

		protected ArgumentNaNException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}