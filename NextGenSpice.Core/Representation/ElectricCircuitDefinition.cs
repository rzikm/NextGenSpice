using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    public class ElectricCircuitDefinition : ICircuitDefinition
    {
        private static CompositionHost compositionHost;
        private static CompositionHost CompositionContainer => compositionHost ?? (compositionHost = GetCompositionContainer());

        public int NodeCount => InitialVoltages.Count;
        public IReadOnlyList<double> InitialVoltages { get; }
        public IReadOnlyList<ICircuitDefinitionElement> Elements { get; }

        private readonly Dictionary<Type, object> factories;

        public ElectricCircuitDefinition(IReadOnlyList<double> initialVoltages, IReadOnlyList<ICircuitDefinitionElement> elements)
        {
            factories = new Dictionary<Type, object>();
            InitialVoltages = initialVoltages;
            Elements = elements;

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
                return (IAnalysisModelFactory<TAnalysisModel>)factory;
            }
            
            if (CompositionContainer.TryGetExport<IAnalysisModelFactory<TAnalysisModel>>(out var export))
            {
                factories[typeof(TAnalysisModel)] = export;
                return export;
            }

            throw new InvalidOperationException($"No Factory for '{typeof(TAnalysisModel).FullName}' was found");
        }

        private static CompositionHost GetCompositionContainer()
        {
            var assemblies = Directory
                .GetFiles(Path.GetDirectoryName(typeof(ElectricCircuitDefinition).Assembly.Location), "NextGenSpice*.dll",
                    SearchOption.AllDirectories);

            var asms = assemblies
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                .ToList();


            var configuration = new ContainerConfiguration().WithAssemblies(asms);
            var compositionHost = configuration.CreateContainer();
            return compositionHost;
        }
    }
}