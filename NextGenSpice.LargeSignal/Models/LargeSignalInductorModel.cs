using System;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public class LargeSignalInductorModel : TwoNodeLargeSignalModel<InductorElement>, ITimeDependentLargeSignalDeviceModel
    {
        private double vc;
        private double vEq;
        private double gEq;

        private int additionalVariable;
        
        public LargeSignalInductorModel(InductorElement parent) : base(parent)
        {
            gEq = gEq = double.PositiveInfinity;
            vEq = parent.InitialVoltage;
        }

        public override void Initialize(IEquationSystemBuilder builder)
        {
            additionalVariable = builder.AddVariable();
            base.Initialize(builder);
        }
        
        public void UpdateTimeDependentModel(SimulationContext context)
        {
            gEq = Parent.Inductance / context.Timestep;
            vEq = gEq * vc;
            vc = context.NodeVoltages[Parent.Anode] - context.NodeVoltages[Parent.Kathode];
        }

        public void ApplyTimeDependentModelValues(IEquationSystem equation, SimulationContext context)
        {
            if (1/gEq < double.Epsilon)
            {
                //TODO: Use different method of calculating?
                 equation.BindEquivalent(Anode, Kathode);
                return;
            }

            equation
                .AddConductance(Anode, Kathode, gEq)
                .AddVoltage(Anode, Kathode, additionalVariable, vEq);
            
        }
    }
}