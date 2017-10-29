namespace NextGenSpice.Core.Elements
{
    public class VoltageSourceElement : TwoNodeCircuitElement
    {
        public double Voltage { get; set; }
        public VoltageSourceElement(double voltage)
        {
            Voltage = voltage;
        }
    }
}