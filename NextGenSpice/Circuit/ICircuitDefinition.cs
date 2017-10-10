namespace NextGenSpice.Circuit
{
    public interface ICircuitDefinition
    {
        ICircuitModel GetLargeSignalModel();
        ICircuitModel GetSmallSignalModel();
    }
}