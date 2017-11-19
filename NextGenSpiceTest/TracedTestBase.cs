using System;
using System.Diagnostics;
using System.Text;
using Xunit.Abstractions;

namespace NextGenSpiceTest
{
    public class TracedTestBase : IDisposable
    {
        protected MyTraceListener traceListener;

        protected class MyTraceListener : TraceListener
        {
            private readonly ITestOutputHelper output;

            private StringBuilder sb = new StringBuilder();

            public MyTraceListener(ITestOutputHelper output)
            {
                this.output = output;
            }

            public override void Write(string message)
            {
                sb.Append(message);
            }

            public override void WriteLine(string message)
            {
                sb.Append(message);
                output.WriteLine(sb.ToString());
                sb.Clear();
            }
        }

        public void Dispose()
        {
            Trace.Listeners.Remove(traceListener);
        }
    }
}