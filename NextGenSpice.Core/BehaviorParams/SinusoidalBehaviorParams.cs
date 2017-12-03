namespace NextGenSpice.Core.BehaviorParams
{
    public class SinusoidalBehaviorParams : SourceBehaviorParams
    {
        public double BaseValue { get; set; }
        public double Amplitude { get; set; }
        public double Frequency { get; set; }
        public double Delay { get; set; }
        public double DampingFactor { get; set; }
        public double Phase { get; set; }
    }
}