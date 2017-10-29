namespace NextGenSpice.Core.Elements
{
    public class CurrentSourceElement : TwoNodeCircuitElement
    {
        public double Current { get; set; }
        public CurrentSourceElement(double current)
        {
            Current = current;
        }
    }
}