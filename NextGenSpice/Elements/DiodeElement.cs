using System;
using NextGenSpice.Circuit;
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

        public override void Accept<T>(ICircuitVisitor<T> visitor)
        {
            throw new NotImplementedException();
        }

        public override ICircuitModelElement GetLargeSignalModel()
        {
            return new DiodeElementModel(this);
        }

        public override ICircuitModelElement GetSmallSignalModel()
        {
            return new DiodeElementModel(this);
        }
    }
}