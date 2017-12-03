using NextGenSpice.Core.BehaviorParams;

namespace NextGenSpice.Core.Elements
{
    public class PulseBehaviorParams : SourceBehaviorParams
    {
        //TODO: Check parameters
        public double Value1 { get; set; }
        public double Value2 { get; set; }
        public double Delay { get; set; }
        public double TimeRise { get; set; }
        public double Duration { get; set; }
        public double TimeFall { get; set; }
        public double Period { get; set; }
    }
}