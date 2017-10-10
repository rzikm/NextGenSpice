namespace NextGenSpice.Circuit
{
    public interface ICircuitDefinition
    {
        ICircuitModel GetDcOperatingPointAnalysisModel();
        ICircuitModel GetTransientAnalysisModel();
    }
}