using System;
using System.Diagnostics;
using System.Text;
using Xunit.Abstractions;

namespace NextGenSpiceTest
{
    public class TracedTestBase : IDisposable
    {

        [ThreadStatic]
        private static ITestOutputHelper output;

        [ThreadStatic]
        private static StringBuilder sb;

        [ThreadStatic]
        protected static bool DoTrace;

        private static MyTraceListener myTraceListener;

        protected ITestOutputHelper Output => output;

        public TracedTestBase(ITestOutputHelper output)
        {
            TracedTestBase.output = output;
            sb = new StringBuilder();
            DoTrace = true;
        }

        static TracedTestBase()
        {
            myTraceListener = new MyTraceListener();
            Trace.Listeners.Add(myTraceListener);
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

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            Trace.Listeners.Remove(myTraceListener);
        }
    }
}