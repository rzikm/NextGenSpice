using NextGenSpice.Models;

namespace NextGenSpice.Elements
{
    public class DiodeElement : TwoNodeCircuitElement
    {
        public readonly DiodeModelParams param;

        public DiodeElement(DiodeModelParams param)
        {
            this.param = param;
        }

        public override ILargeSignalDeviceModel GetLargeSignalModel()
        {
            return new DiodeModel(this);
        }

        public override ILargeSignalDeviceModel GetSmallSignalModel()
        {
            return new DiodeModel(this);
        }
    }
}