using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalSubcircuitModel : LargeSignalModelBase<SubcircuitElement>, ILinearLargeSignalDeviceModel,
        INonlinearLargeSignalDeviceModel, ITimeDependentLargeSignalDeviceModel
    {
        private readonly BiasedEquationEditor biasedEquationEditor;
        private readonly BiasedSimulationContext subContext;
        private readonly int[] nodeMap;

        private readonly ILargeSignalDeviceModel[] elements;

        private readonly ILinearLargeSignalDeviceModel[] linearModels;
        private readonly INonlinearLargeSignalDeviceModel[] nonlinearModels;
        private readonly ITimeDependentLargeSignalDeviceModel[] timeDependentModels;

        public IReadOnlyList<ILargeSignalDeviceModel> Elements => elements;

        public LargeSignalSubcircuitModel(SubcircuitElement parent, IEnumerable<ILargeSignalDeviceModel> elements) :
            base(parent)
        {
            this.elements = elements.ToArray();

            linearModels = this.elements.OfType<ILinearLargeSignalDeviceModel>().ToArray();
            nonlinearModels = this.elements.OfType<INonlinearLargeSignalDeviceModel>().ToArray();
            timeDependentModels = this.elements.OfType<ITimeDependentLargeSignalDeviceModel>().ToArray();

            nodeMap = new int[parent.InnerNodeCount + 1];
            biasedEquationEditor = new BiasedEquationEditor(nodeMap);
            subContext = new BiasedSimulationContext(nodeMap);
        }

        public override void PostProcess(ISimulationContext context)
        {
            base.PostProcess(context);
            subContext.TrueContext = context;

            foreach (var model in elements)
                model.PostProcess(context);
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            base.Initialize(builder);

            for (var i = 1; i < nodeMap.Length; i++)
                nodeMap[i] = builder.AddVariable();

            biasedEquationEditor.TrueEquationEditor = builder;

            foreach (var model in elements)
                model.Initialize(builder);
        }

        public void ApplyLinearModelValues(IEquationEditor equation, ISimulationContext context)
        {
            biasedEquationEditor.TrueEquationEditor = equation;
            subContext.TrueContext = context;

            foreach (var model in linearModels)
                model.ApplyLinearModelValues(biasedEquationEditor, context);
        }

        public void UpdateNonlinearModel(ISimulationContext context)
        {
            subContext.TrueContext = context;

            foreach (var model in nonlinearModels)
                model.UpdateNonlinearModel(context);
        }

        public void ApplyNonlinearModelValues(IEquationSystem equation, ISimulationContext context)
        {
            subContext.TrueContext = context;
            biasedEquationEditor.TrueEquationEditor = equation;

            foreach (var model in nonlinearModels)
                model.ApplyNonlinearModelValues(biasedEquationEditor, context);
        }

        public void UpdateTimeDependentModel(ISimulationContext context)
        {
            subContext.TrueContext = context;

            foreach (var model in timeDependentModels)
                model.UpdateTimeDependentModel(context);
        }

        public void RollbackTimeDependentModel()
        {
            foreach (var model in timeDependentModels)
                model.RollbackTimeDependentModel();
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, ISimulationContext context)
        {
            biasedEquationEditor.TrueEquationEditor = equation;
            subContext.TrueContext = context;

            foreach (var model in timeDependentModels)
                model.ApplyTimeDependentModelValues(biasedEquationEditor, context);
        }
    }
}