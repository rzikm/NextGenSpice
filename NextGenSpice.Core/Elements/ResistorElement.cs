namespace NextGenSpice.Core.Elements
{
    public class ResistorElement : TwoNodeCircuitElement
    {
        public double Resistance { get; set; }

        public ResistorElement(double resistance)
        {
            this.Resistance = resistance;
        }
    }
}