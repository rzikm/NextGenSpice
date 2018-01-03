using NextGenSpice.LargeSignal;

namespace NextGenSpice
{
    public abstract class LsPrintStatement : PrintStatement<LargeSignalCircuitModel>
    {
        public string AnalysisType { get; set; }
    }
}