using NextGenSpice.Circuit;

namespace NextGenSpice.Elements
{
    public interface ICircuitDefinitionElement
    {
        NodeConnectionSet ConnectedNodes { get; }

        ILargeSignalDeviceModel GetLargeSignalModel();
        ILargeSignalDeviceModel GetSmallSignalModel();
    }
}