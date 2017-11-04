namespace NextGenSpice.Core.Elements
{
    public class DiodeModelParams
    {
        public static DiodeModelParams Default => new DiodeModelParams();

        private DiodeModelParams()
        {
            
        }

        public double IS = 1e-15;
        public double Vt = 0.025875;
        public double Vd = 0.9;
    }
}