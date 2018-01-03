namespace NextGenSpice
{
    public class PrintStatementDeprecated
    {
        public string AnalysisType { get; set; }
        public Token Token { get; set; }
    }

    public abstract class PrintStatement<TModel> : PrintStatement
    {
        protected PrintStatement()
        {
        }

        public override void Initialize(object model)
        {
            Initialize((TModel)model);
        }

        public abstract void Initialize(TModel model);
    }
}