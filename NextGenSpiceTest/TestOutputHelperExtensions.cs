using NextGenSpice.LargeSignal;
using Xunit.Abstractions;

namespace NextGenSpiceTest
{
    public static class TestOutputHelperExtensions
    {
        public static void PrintCircuitStats(this ITestOutputHelper output, LargeSignalCircuitModel model)
        {
            output.WriteLine($"Iterations: {model.IterationCount}");
            output.WriteLine($"Delta^2: {model.DeltaSquared}");
            output.WriteLine("Voltages:");
            for (var id = 0; id < model.NodeVoltages.Length; id++)
            {
                output.WriteLine($"[{id}]:\t{model.NodeVoltages[id]}");
            }
            output.WriteLine("");
        }
    }
}