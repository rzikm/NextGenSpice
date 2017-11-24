namespace NextGenSpice.Core.Elements
{
    public class DiodeElement : TwoNodeCircuitElement
    {
        public readonly DiodeModelParams param;

        public DiodeElement(DiodeModelParams param, string name = null) : base(name)
        {
            this.param = param;
        }
    }
}