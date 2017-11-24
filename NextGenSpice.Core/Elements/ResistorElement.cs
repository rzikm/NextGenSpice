namespace NextGenSpice.Core.Elements
{
    public class ResistorElement : TwoNodeCircuitElement
    {
        public double Resistance { get; set; }

        public ResistorElement(double resistance, string name = null) : base(name)
        {
            this.Resistance = resistance;
        }
    }
}