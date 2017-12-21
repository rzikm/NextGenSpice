using System.Collections.Generic;

namespace NextGenSpice
{
    public class ModelStatement : DeferredStatement
    {
        public ModelType Type { get; set; }
        public string Name { get; set; }
        public object Model { get; set; }
        public override bool CanApply(ParsingContext context)
        {
            return true;
        }

        public override IEnumerable<ErrorInfo> GetErrors()
        {
            throw new System.NotImplementedException();
        }

        public override void Apply(ParsingContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}