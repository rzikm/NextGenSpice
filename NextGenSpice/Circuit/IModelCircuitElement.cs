namespace NextGenSpice.Circuit
{
    public interface IModelCircuitElement
    {
        CircuitNode[] Connections { get; }
    }
}