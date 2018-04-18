using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace NextGenSpice.Core.Representation
{
    public class AnalysisModelCreator : IAnalysisModelCreator
    {
        private static AnalysisModelCreator instance;
        public static AnalysisModelCreator Instance => instance ?? (instance = new AnalysisModelCreator());

        private static CompositionHost compositionHost;
        private readonly Dictionary<Type, object> factories;

        public AnalysisModelCreator()
        {
            factories = new Dictionary<Type, object>();
        }

        private static CompositionHost CompositionContainer =>
            compositionHost ?? (compositionHost = GetCompositionContainer());

        /// <summary>Sets factory for creating a model for specific analysis.</summary>
        /// <typeparam name="TAnalysisModel"></typeparam>
        /// <param name="factory">Instance of factory that creates the analysis-specific model instance.</param>
        public void SetFactory<TAnalysisModel>(IAnalysisModelFactory<TAnalysisModel> factory)
        {
            factories[typeof(TAnalysisModel)] = factory;
        }

        /// <summary>Creates analysis-specific model of given type using registered factory instance.</summary>
        /// <typeparam name="TAnalysisModel">Analysis-specific model type.</typeparam>
        /// <param name="circuitDefinition">Definition of the circuit, whose analysis model should be created</param>
        /// <returns></returns>
        public TAnalysisModel GetModel<TAnalysisModel>(ICircuitDefinition circuitDefinition)
        {
            var factory = GetFactory<TAnalysisModel>();
            return factory.Create(circuitDefinition);
        }

        /// <summary>Gets the instance of factory class responsible for creating analysis-specific model of givent type.</summary>
        /// <typeparam name="TAnalysisModel">Analysis-specific model type.</typeparam>
        /// <returns></returns>
        public IAnalysisModelFactory<TAnalysisModel> GetFactory<TAnalysisModel>()
        {
            if (factories.TryGetValue(typeof(TAnalysisModel), out var factory))
                return (IAnalysisModelFactory<TAnalysisModel>)factory;

            if (CompositionContainer.TryGetExport<IAnalysisModelFactory<TAnalysisModel>>(out var export))
            {
                factories[typeof(TAnalysisModel)] = export;
                return export;
            }

            throw new InvalidOperationException($"No Factory for '{typeof(TAnalysisModel).FullName}' was found");
        }

        /// <summary>Gets MEF composition container with all exports.</summary>
        /// <returns></returns>
        private static CompositionHost GetCompositionContainer()
        {
            //TODO: allow other dlls?
            var assemblies = Directory
                .GetFiles(Path.GetDirectoryName(typeof(CircuitDefinition).Assembly.Location),
                    "NextGenSpice*.dll",
                    SearchOption.AllDirectories);

            var asms = assemblies
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath)
                .ToList();

            var configuration = new ContainerConfiguration().WithAssemblies(asms);
            return configuration.CreateContainer();
        }
    }
}