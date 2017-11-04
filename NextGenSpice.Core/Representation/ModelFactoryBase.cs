using System;

namespace NextGenSpice.Core.Representation
{
    public abstract class ModelFactoryBase 
    {
        protected ModelFactoryBase(Type analysisModelType)
        {
            AnalysisModelType = analysisModelType;
        }

        public Type AnalysisModelType { get; }
    }
}