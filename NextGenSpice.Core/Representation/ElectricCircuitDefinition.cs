using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using NextGenSpice.Core.Elements;

namespace NextGenSpice.Core.Representation
{
    /// <summary>
    ///     Class that represents definition of an electric circuit.
    /// </summary>
    public class ElectricCircuitDefinition : ICircuitDefinition
    {
        private static CompositionHost compositionHost;

        private readonly Dictionary<Type, object> factories;

        public ElectricCircuitDefinition(IReadOnlyList<double?> initialVoltages,
            IReadOnlyList<ICircuitDefinitionElement> elements)
        {
            factories = new Dictionary<Type, object>();
            InitialVoltages = initialVoltages;
            Elements = elements;
        }

        private static CompositionHost CompositionContainer =>
            compositionHost ?? (compositionHost = GetCompositionContainer());

        /// <summary>
        ///     Number of the nodes in the circuit.
        /// </summary>
        public int NodeCount => InitialVoltages.Count;

        /// <summary>
        ///     Initial voltages of nodes by their id.
        /// </summary>
        public IReadOnlyList<double?> InitialVoltages { get; }

        /// <summary>
        ///     Set of elements that define this circuit.
        /// </summary>
        public IReadOnlyList<ICircuitDefinitionElement> Elements { get; }

        /// <summary>
        ///     Sets factory for creating a model for specific analysis.
        /// </summary>
        /// <typeparam name="TAnalysisModel"></typeparam>
        /// <param name="factory">Instance of factory that creates the analysis-specific model instance.</param>
        public void SetFactory<TAnalysisModel>(IAnalysisModelFactory<TAnalysisModel> factory)
        {
            factories[typeof(TAnalysisModel)] = factory;
        }

        /// <summary>
        ///     Creates analysis-specific model of given type using registered factory instance.
        /// </summary>
        /// <typeparam name="TAnalysisModel">Analysis-specific model type.</typeparam>
        /// <returns></returns>
        public TAnalysisModel GetModel<TAnalysisModel>()
        {
            var factory = GetFactory<TAnalysisModel>();
            return factory.Create(this);
        }

        /// <summary>
        ///     Gets the instance of factory class responsible for creating analysis-specific model of givent type.
        /// </summary>
        /// <typeparam name="TAnalysisModel">Analysis-specific model type.</typeparam>
        /// <returns></returns>
        public IAnalysisModelFactory<TAnalysisModel> GetFactory<TAnalysisModel>()
        {
            if (factories.TryGetValue(typeof(TAnalysisModel), out var factory))
                return (IAnalysisModelFactory<TAnalysisModel>) factory;

            if (CompositionContainer.TryGetExport<IAnalysisModelFactory<TAnalysisModel>>(out var export))
            {
                factories[typeof(TAnalysisModel)] = export;
                return export;
            }

            throw new InvalidOperationException($"No Factory for '{typeof(TAnalysisModel).FullName}' was found");
        }

        /// <summary>
        ///     Gets MEF composition container with all exports.
        /// </summary>
        /// <returns></returns>
        private static CompositionHost GetCompositionContainer()
        {
            //TODO: allow other dlls?
            var assemblies = Directory
                .GetFiles(Path.GetDirectoryName(typeof(ElectricCircuitDefinition).Assembly.Location),
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