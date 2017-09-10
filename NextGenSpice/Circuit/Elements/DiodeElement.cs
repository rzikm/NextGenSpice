using System;

namespace NextGenSpice.Circuit
{
    public class DiodeElement : TwoNodeCircuitElement
    {
        public readonly DiodeModelParams param;

        public DiodeElement(DiodeModelParams param)
        {
            this.param = param;
        }

        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override ICircuitModelElement GetDcOperatingPointModel()
        {
            return new DiodeElementModel(this);
        }

        public override ICircuitModelElement GetTransientModel()
        {
            return new DiodeElementModel(this);
        }
    }
}