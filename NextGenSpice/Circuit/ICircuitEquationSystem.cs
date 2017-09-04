namespace NextGenSpice.Circuit
{
    public interface ICircuitEquationSystem
    {
        void AddConductance(int fromId, int toId, double value);
        void AddCurrent(int nodeId, double valueId);
        void MergeNodes(int n1, int n2);
        double[] NodeVoltages { get; }
        void Clear();
        void Solve();
    }
}