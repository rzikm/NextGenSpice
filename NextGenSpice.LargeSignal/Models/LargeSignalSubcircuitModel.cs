using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalSubcircuitModel : LargeSignalModelBase<SubcircuitElement>
    {
        private readonly BiasedEquationEditor biasedEquationEditor;
        private readonly BiasedSimulationContext subContext;
        private readonly int[] nodeMap;

        private readonly ILargeSignalDeviceModel[] elements;

        public IReadOnlyList<ILargeSignalDeviceModel> Elements => elements;

        public LargeSignalSubcircuitModel(SubcircuitElement parent, IEnumerable<ILargeSignalDeviceModel> elements) :
            base(parent)
        {
            this.elements = elements.ToArray();

            nodeMap = new int[parent.InnerNodeCount + 1];
            biasedEquationEditor = new BiasedEquationEditor(nodeMap);
            subContext = new BiasedSimulationContext(nodeMap);
        }

        public override void OnDcBiasEstablished(ISimulationContext context)
        {
            base.OnDcBiasEstablished(context);
            subContext.TrueContext = context;

            foreach (var model in elements)
                model.OnDcBiasEstablished(context);
        }

        public override void RegisterAdditionalVariables(IEquationSystemBuilder builder)
        {
            base.RegisterAdditionalVariables(builder);

            for (var i = 1; i < nodeMap.Length; i++)
                nodeMap[i] = -1;

            for (int i = 0; i < Parent.TerminalNodes.Length; i++)
                nodeMap[Parent.TerminalNodes[i]] = Parent.ConnectedNodes[i];

            for (var i = 1; i < nodeMap.Length; i++)
                nodeMap[i] = nodeMap[i] < 0 ? builder.AddVariable() : nodeMap[i];

            biasedEquationEditor.TrueEquationEditor = builder;

            foreach (var model in elements)
                model.RegisterAdditionalVariables(builder);
        }

        public override void ApplyModelValues(IEquationEditor equations, ISimulationContext context)
        {
            biasedEquationEditor.TrueEquationEditor = equations;
            subContext.TrueContext = context;

            foreach (var model in elements)
                model.ApplyModelValues(biasedEquationEditor, context);
        }
    }
}