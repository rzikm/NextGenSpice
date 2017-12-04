namespace NextGenSpice.Core.BehaviorParams
{
    public class AmBehaviorParams : SourceBehaviorParams
    {
        public double SignalAmplitude { get; set; }
        public double FrequencyCarrier { get; set; }
        public double FrequencyModulation { get; set; }
        public double Offset { get; set; }
        public double Delay { get; set; }
    }
}