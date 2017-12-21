namespace NextGenSpice
{
    public abstract class SimpleStatementProcessor<TParam> : StatementProcessor
    {
        protected ParameterMapper<TParam> Mapper { get; }

        public SimpleStatementProcessor()
        {
            Mapper = new ParameterMapper<TParam>();
        }

        protected abstract void InitMapper();

        protected override void DoProcess(Token[] tokens)
        {
            InitMapper();
            for (int i = 1; i < tokens.Length; i++)
            {
                Mapper.Set(i, tokens[i].GetNumericValue(Context.Errors));
            }
            UseParam();
        }

        protected abstract void UseParam();
    }
}