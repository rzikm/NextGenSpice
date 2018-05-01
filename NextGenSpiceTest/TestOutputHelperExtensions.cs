using NextGenSpice.LargeSignal;
using Xunit.Abstractions;

namespace NextGenSpice.Test
{
    public static class TestOutputHelperExtensions
    {
        public static void PrintCircuitStats(this ITestOutputHelper output, LargeSignalCircuitModel model)
        {
            output.WriteLine($"Iterations: {model.LastNonLinearIterationCount}");
            output.WriteLine($"Delta: {model.LastNonLinearIterationDelta}");
            output.WriteLine("Voltages:");
            for (var id = 0; id < model.NodeVoltages.Length; id++)
                output.WriteLine($"[{id}]:\t{model.NodeVoltages[id]}");
            output.WriteLine("");
        }
    }
}