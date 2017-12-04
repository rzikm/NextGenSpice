namespace NextGenSpice.Core.BehaviorParams
{
    public class SffmBehaviorParams : SourceBehaviorParams
    {
        public double BaseValue { get; set; }
        public double Amplitude { get; set; }
        public double FrequencyCarrier { get; set; }
        public double FrequencySignal { get; set; }
        public double ModilationIndex { get; set; }
    }
}