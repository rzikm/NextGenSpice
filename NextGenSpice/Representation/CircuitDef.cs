using System;
using System.Collections.Generic;
using NextGenSpice.Circuit;
using NextGenSpice.Elements;

namespace NextGenSpice.Representation
{
    public class CircuitDef : ICircuitDefinition2
    {
        public List<CircuitNode> Nodes { get; set; }
        public List<ICircuitDefinitionElement> Elements { get; set; }

        private readonly Dictionary<Type, object> factories;

        public CircuitDef()
        {
            factories = new Dictionary<Type, object>();
        }

        public void SetFactory<TAnalysisModel>(IAnalysisModelFactory<TAnalysisModel> factory)
        {
            factories[typeof(TAnalysisModel)] = factory;
        }

        public TAnalysisModel GetModel<TAnalysisModel>()
        {
            IAnalysisModelFactory<TAnalysisModel> factory = GetFactory<TAnalysisModel>();
            return factory.Create(this);
        }

        public IAnalysisModelFactory<TAnalysisModel> GetFactory<TAnalysisModel>()
        {
            if (factories.TryGetValue(typeof(TAnalysisModel), out var factory))
            {
                return (IAnalysisModelFactory<TAnalysisModel>) factory;
            }
            throw new InvalidOperationException();
        }


        public ICircuitModel GetLargeSignalModel()
        {
            throw new NotImplementedException();
        }

        public ICircuitModel GetSmallSignalModel()
        {
            throw new NotImplementedException();
        }
    }
}