using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Numerics.Equations.Eq;
using NextGenSpice.Numerics.Precision;

namespace NextGenSpice.LargeSignal
{
    /// <summary>Main class for performing large signal analysis on electrical circuits.</summary>
    public class LargeSignalCircuitModel : IAnalysisCircuitModel<ILargeSignalDevice>
    {
        private SimulationContext context;

        private double[] currentSolution;
        private double[] previousSolution;

        private EquationSystemAdapter equationSystemAdapter;

        public LargeSignalCircuitModel(IEnumerable<double?> initialVoltages, List<ILargeSignalDevice> devices)
        {
            this.initialVoltages = initialVoltages.ToArray();
            NodeVoltages = new double[this.initialVoltages.Length];
            this.devices = devices.ToArray();

//            deviceLookup = devices.Where(e => !string.IsNullOrEmpty(e.DefinitionDevice.Name)).ToDictionary(e => e.DefinitionDevice.Name);
            deviceLookup = devices.Where(e => e.DefinitionDevice.Tag != null).ToDictionary(e => e.DefinitionDevice.Tag);
            updaterInitCondition = e => e.ApplyInitialCondition(context);
            updaterModelValues = e => e.ApplyModelValues(context);
        }

        /// <summary>Last computed node voltages.</summary>
        public double[] NodeVoltages { get; }

        /// <summary>Number of node in the circuit.</summary>
        public int NodeCount => NodeVoltages.Length;

        /// <summary>Set of all devices that this circuit model consists of.</summary>
        public IReadOnlyList<ILargeSignalDevice> Devices => devices;

        /// <summary>
        /// Returns device with given tag or null if no such device exists.
        /// </summary>
        /// <param name="tag">The tag of the queried device.</param>
        /// <returns></returns>
        public ILargeSignalDevice FindDevice(object tag)
        {
            deviceLookup.TryGetValue(tag, out var ret);
            return ret;
        }

        private ILargeSignalDevice[] constDevices;
        private ILargeSignalDevice[] nonlinearDevices;
        private ILargeSignalDevice[] linearTimeDependentDevices;

        private readonly Dictionary<object, ILargeSignalDevice> deviceLookup;
        private readonly double?[] initialVoltages;
        private readonly ILargeSignalDevice[] devices;

        // cached functors to prevent excessive allocations
        private readonly Action<ILargeSignalDevice> updaterInitCondition;
        private readonly Action<ILargeSignalDevice> updaterModelValues;

        /// <summary>Gets model for the device identified by given name.</summary>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public ILargeSignalDevice GetDevice(string name)
        {
            return deviceLookup[name];
        }

        /// <summary>Gets model for the device identified by given name.</summary>
        /// <param name="name">Name of the device.</param>
        /// <param name="value">The device model.</param>
        /// <returns></returns>
        public bool TryGetDevice(string name, out ILargeSignalDevice value)
        {
            return deviceLookup.TryGetValue(name, out value);
        }

        private bool IsLinear => nonlinearDevices.Length == 0;


        // iteration dependent variables

        /// <summary>
        ///     Minimum absolute difference between two consecutive Newton-Raphson iterations before stable solution is
        ///     assumed.
        /// </summary>
        public double NonlinearIterationEpsilon { get; set; } = 1e-4;

        /// <summary>Maximumum number of Newton-Raphson iterations per timepoint.</summary>
        public int MaxDcPointIterations { get; set; } = 1000;

        /// <summary>Maximal timestep between two time points during transient analysis.</summary>
        public double MaxTimeStep { get; set; } = 1e-6;

        /// <summary>How many Newton-Raphson iterations were needed in last operating point calculation.</summary>
        public int LastNonLinearIterationCount { get; private set; }

        /// <summary>Difference between last two Newton-Raphson solutions during last operating point calculation.</summary>
        public double LastNonLinearIterationDelta { get; private set; }

        /// <summary>Current timepoint of the transient analysis in seconds.</summary>
        public double CurrentTimePoint => context?.TimePoint ?? 0.0;

        /// <summary>
        ///     Advances transient simulation of the circuit by given ammount in seconds, respecting the maximum allowed
        ///     timestep.
        /// </summary>
        /// <param name="timestep"></param>
        public void AdvanceInTime(double timestep)
        {
            if (timestep < 0) throw new ArgumentOutOfRangeException(nameof(timestep));
            if (context == null) EstablishInitialDcBias();

            while (timestep > 0)
            {
                context.TimePoint = context.TimePoint + timestep;
                context.TimeStep = timestep;
                EstablishDcBias_Internal(updaterModelValues);
                OnDcBiasEstablished();
                timestep -= timestep;
            }
        }

        /// <summary>Establishes initial operating point for the transient analysis.</summary>
        public void EstablishInitialDcBias(bool initCond = false)
        {
            context = null; // reset;
            EnsureInitialized();
            //            context.TimeStep = 0;
            //            context.TimePoint = 0;

//            for (int i = 0; i < initialVoltages.Length; i++)
//            {
//                if (initialVoltages[i].HasValue) // initial condition
//                {
//                    context.EquationSystem
//                        .AddCurrent(i, 0, initialVoltages[i].Value)
//                        .AddConductance(i, 0, 1);
//                }
//            }

            if (!EstablishDcBias_Internal(updaterInitCondition))
                throw new NonConvergenceException();

            // rerun without initial voltages
            if (!EstablishDcBias_Internal(updaterInitCondition))
                throw new NonConvergenceException();

            OnDcBiasEstablished();
        }

        private void EnsureInitialized()
        {
            constDevices = Devices.Where(e => e.UpdateMode == ModelUpdateMode.NoUpdate).ToArray();
            linearTimeDependentDevices = Devices.Where(e => e.UpdateMode == ModelUpdateMode.TimePoint).ToArray();
            nonlinearDevices = Devices.Where(e => e.UpdateMode == ModelUpdateMode.Always).ToArray();

            if (context != null) return;

            context = new SimulationContext(NodeCount);
            BuildEquationSystem();
        }

        private void BuildEquationSystem()
        {
            equationSystemAdapter = new EquationSystemAdapter(NodeCount);

            foreach (var device in Devices)
                device.Initialize(equationSystemAdapter, context);

            equationSystemAdapter.Freeze();
            
            currentSolution = new double[equationSystemAdapter.VariableCount];
            previousSolution = new double[equationSystemAdapter.VariableCount];
        }

        private bool EstablishDcBias_Internal(Action<ILargeSignalDevice> updater)
        {
            LastNonLinearIterationCount = 0;
            LastNonLinearIterationDelta = 0;

            UpdateEquationSystem(updater, devices);

            SolveAndUpdateVoltages();
            //            DebugPrint();
            return IsLinear || IterateUntilConvergence(updater);
        }

        private bool IterateUntilConvergence(Action<ILargeSignalDevice> updater)
        {
            double delta;

            do
            {
                delta = 0;

                UpdateEquationSystem(updater, devices);
                SolveAndUpdateVoltages();
                //            DebugPrint();

                for (var i = 0; i < previousSolution.Length; i++)
                {
                    var d = previousSolution[i] - currentSolution[i];
                    delta += d * d;
                }

                if (++LastNonLinearIterationCount == MaxDcPointIterations) return false;
            } while (delta > NonlinearIterationEpsilon * NonlinearIterationEpsilon);

            LastNonLinearIterationDelta = Math.Sqrt(delta);

            return true;
        }

        private void OnDcBiasEstablished()
        {
            for (var i = 0; i < devices.Length; i++)
                devices[i].OnDcBiasEstablished(context);
        }

        private void SolveAndUpdateVoltages()
        {
            // ensure ground has 0 voltage
            equationSystemAdapter.Anullate();

            var tmp = currentSolution;
            currentSolution = previousSolution;
            previousSolution = tmp;
            equationSystemAdapter.Solve(currentSolution);

            for (var i = 0; i < NodeCount; i++)
                NodeVoltages[i] = currentSolution[i];
        }

        private void UpdateEquationSystem(Action<ILargeSignalDevice> updater,
            ILargeSignalDevice[] elem)
        {
            equationSystemAdapter.Clear();
            for (var i = 0; i < elem.Length; i++)
                updater(elem[i]);
        }

        private class SimulationContext : ISimulationContext
        {
            public SimulationContext(int nodeCount)
            {
                NodeCount = nodeCount;
                CircuitParameters = new CircuitParameters();
            }

            public double NodeCount { get; }
            public double TimePoint { get; set; }
            public double TimeStep { get; set; }

            public CircuitParameters CircuitParameters { get; }
        }
    }
}