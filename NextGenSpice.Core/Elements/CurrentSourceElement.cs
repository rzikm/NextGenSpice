namespace NextGenSpice.Core.Elements
{
    public class CurrentSourceElement : TwoNodeCircuitElement
    {
        public double Current { get; set; }
        public CurrentSourceElement(double current, string name = null) : base(name)
        {
            Current = current;
        }
    }
}