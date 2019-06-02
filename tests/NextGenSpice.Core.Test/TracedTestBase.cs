using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using NextGenSpice.Parser;
using Xunit.Abstractions;

namespace NextGenSpice.Core.Test
{
	public class TracedTestBase : IDisposable
	{
		[ThreadStatic] private static ITestOutputHelper output;

		[ThreadStatic] private static StringBuilder sb;

		[ThreadStatic] protected static bool DoTrace;

		private static readonly MyTraceListener myTraceListener;

		static TracedTestBase()
		{
			myTraceListener = new MyTraceListener();
			Trace.Listeners.Add(myTraceListener);
		}

		public TracedTestBase(ITestOutputHelper output)
		{
			TracedTestBase.output = output;
			sb = new StringBuilder();
			DoTrace = true;
		}

		protected ITestOutputHelper Output => output;

		/// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
		public virtual void Dispose()
		{
			Trace.Listeners.Remove(myTraceListener);
		}

		public SpiceNetlistParserResult Parse(string code)
		{
			var parser = SpiceNetlistParser.WithDefaults();
			var result = parser.Parse(new StringReader(code));
			Output.WriteLine(string.Join("\n", result.Errors));
			return result;
		}

		private class MyTraceListener : TraceListener
		{
			public override void Write(string message)
			{
				Console.Write(message);
				sb?.Append(message);
			}

			public override void WriteLine(string message)
			{
				Console.WriteLine(message);
				if (sb == null || !DoTrace) return;
				sb.Append(message);
				output.WriteLine(sb.ToString());
				sb.Clear();
			}
		}
	}
}