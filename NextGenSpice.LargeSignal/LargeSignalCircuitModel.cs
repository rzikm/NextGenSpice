﻿using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Models;
using Numerics;

namespace NextGenSpice.LargeSignal
{
    public class LargeSignalCircuitModel : IAnalysisCircuitModel<ILargeSignalDeviceModel>
    {
        private ISimulationContext context;
        private EquationSystem equationSystem;

        public LargeSignalCircuitModel(IEnumerable<double> initialVoltages, List<ILargeSignalDeviceModel> elements)
        {
            NodeVoltages = initialVoltages.ToArray();
            Elements = elements;
        }

        public double[] NodeVoltages { get; }
        public int NodeCount => NodeVoltages.Length;
        public IReadOnlyList<ILargeSignalDeviceModel> Elements { get; }
        public bool IsLinear => false;


        // iteration dependent variables

        public double NonlinearIterationEpsilon { get; } = 1e-15;

        public int MaxDcPointIterations { get; set; } = 1000;
        public double MaxTimeStep { get; set; } = 1e-6;

        public double TimestepAbsoluteEpsilon { get; set; }
        public double TimestepRelativeEpsilon { get; set; }


        public int IterationCount { get; private set; }
        public double DeltaSquared { get; private set; }


        public void Simulate(Action<double[]> callback)
        {
            EnsureInitialized();

            while (true)
            {
                EstablishDcBias_Internal(e => e.ApplyModelValues(equationSystem, context));
                callback(NodeVoltages);
            }
        }

        public void AdvanceInTime(double milliseconds)
        {
            if (milliseconds < 0) throw new ArgumentOutOfRangeException(nameof(milliseconds));
            EnsureInitialized();
            while (milliseconds > 0)
            {
                var step = Math.Min(MaxTimeStep, milliseconds);
                milliseconds -= step;

                AdvanceInTime_Internal(step);
            }
        }

        private void AdvanceInTime_Internal(double step)
        {
            context.Timestep = step;
            
            EstablishDcBias_Internal(e => e.ApplyModelValues(equationSystem, context));

            context.Time += step;
        }

        private void EnsureInitialized()
        {
            if (context != null) return;

            BuildEquationSystem();
            context = new SimulationContext(NodeCount, equationSystem);
        }

        private void BuildEquationSystem()
        {
            var b = new EquationSystemBuilder();
            for (var i = 0; i < NodeCount; i++)
                b.AddVariable();

            foreach (var element in Elements)
                element.RegisterAdditionalVariables(b);

            equationSystem = b.Build();
        }

        public void EstablishDcBias()
        {
            EnsureInitialized();

            // TODO: really reinitialize?
            context.Timestep = 0;
            context.Time = 0;

            if (!EstablishDcBias_Internal(e => e.ApplyInitialCondition(equationSystem, context)))
                throw new NonConvergenceException();
        }

        private bool EstablishDcBias_Internal(Action<ILargeSignalDeviceModel> updater)
        {
            IterationCount = 0;
            DeltaSquared = 0;


            Iterate_DcBias(updater);
            if (!IsLinear && !IterateUntilConvergence(updater))
                return false;

            PostProcess();

            return true;
        }

        private bool IterateUntilConvergence(Action<ILargeSignalDeviceModel> updater)
        {
            double delta;
            
            do {
                delta = 0;
                var prevVoltages = (double[]) equationSystem.Solution.Clone();

                Iterate_DcBias(updater);

                for (var i = 0; i < prevVoltages.Length; i++)
                {
                    var d = prevVoltages[i] - equationSystem.Solution[i];
                    delta += d * d;
                }

                if (++IterationCount == MaxDcPointIterations) return false;
            } while (delta > NonlinearIterationEpsilon * NonlinearIterationEpsilon);

            DeltaSquared = delta;

            return true;
        }


        private void Iterate_DcBias(Action<ILargeSignalDeviceModel> updater)
        {
            UpdateEquationSystem(updater);
            UpdateNodeValues();
//            DebugPrint();
        }

        private void PostProcess()
        {
            foreach (var el in Elements)
                el.OnDcBiasEstablished(context);
        }

        private void UpdateNodeValues()
        {
            // ensure ground has 0 voltage
            var m = equationSystem.Matrix;
            for (int i = 0; i < m.SideLength; i++)
            {
                m[i, 0] = 0;
                m[0, i] = 0;
            }

            m[0, 0] = 1;
            equationSystem.RightHandSide[0] = 0;

            equationSystem.Solve();

            for (var i = 0; i < NodeCount; i++)
                NodeVoltages[i] = equationSystem.Solution[i];
        }

        private void UpdateEquationSystem(Action<ILargeSignalDeviceModel> updater)
        {
            equationSystem.Clear();

            foreach (var e in Elements)
                updater(e);
        }

        private void DebugPrint()
        {
            Console.WriteLine("Results:");
            for (var i = 0; i < NodeCount; i++)
            {
                var v = equationSystem.Solution[i];
                Console.WriteLine($"node {i}: {v:##.0000}");
            }
            Console.WriteLine();
        }
    }
}