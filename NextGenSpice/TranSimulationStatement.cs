namespace NextGenSpice
{
    public class TranSimulationStatement
    {
        public string AnalysisType { get; set; }
        public double StopTime { get; set; }
        public double TimeStep { get; set; }
        public double TMax { get; set; }
        public double StartTime { get; set; }
    }
}