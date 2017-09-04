namespace NextGenSpice.Circuit
{
    public class CircuitNode
    {
        public double Voltage { get; set; }
        public int Id { get; internal set; }
        public string Tag { get; set; }
    }
}