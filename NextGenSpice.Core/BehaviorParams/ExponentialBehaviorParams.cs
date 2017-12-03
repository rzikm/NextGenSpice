namespace NextGenSpice.Core.BehaviorParams
{
    public class ExponentialBehaviorParams : SourceBehaviorParams
    {
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double RiseDelay { get; set; }
        public double TauRise { get; set; }
        public double FallDelay { get; set; }
        public double TauFall { get; set; }
    }
}