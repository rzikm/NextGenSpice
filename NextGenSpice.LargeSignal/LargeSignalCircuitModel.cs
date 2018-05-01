using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Numerics.Precision;

namespace NextGenSpice.LargeSignal
{
    /// <summary>Main class for performing large signal analysis on electrical circuits.</summary>
    public class LargeSignalCircuitModel : IAnalysisCircuitModel<ILargeSignalDevice>
    {
        private SimulationContext context;

        /// <summary>
        /// Set of device independent circuit parameters.
        /// </summary>
        public CircuitParameters CircuitParameters { get; }

        private double[] currentSolution;
        private double[] previousSolution;

        private EquationSystemAdapter equationSystemAdapter;

        public LargeSignalCircuitModel(IEnumerable<double?> initialVoltages, List<ILargeSignalDevice> devices)
        {
            this.initialVoltages = initialVoltages.ToArray();
            NodeVoltages = new double[this.initialVoltages.Length];
            this.devices = devices.ToArray();
            initVoltProxies = new List<IEquationSystemCoefficientProxy>();
            CircuitParameters = new CircuitParameters();

            deviceLookup = devices.Where(e => e.DefinitionDevice.Tag != null).ToDictionary(e => e.DefinitionDevice.Tag);
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

        private readonly Dictionary<object, ILargeSignalDevice> deviceLookup;
        private readonly double?[] initialVoltages;
        private readonly List<IEquationSystemCoefficientProxy> initVoltProxies;
        private readonly ILargeSignalDevice[] devices;

        /// <summary>
        ///     Minimum absolute difference between two consecutive Newton-Raphson iterations before stable solution is
        ///     assumed.
        /// </summary>
        public double NonlinearIterationEpsilon { get; set; } = 1e-4;

        /// <summary>Maximumum number of Newton-Raphson iterations per timepoint.</summary>
        public int MaxDcPointIterations { get; set; } = 1000;

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
            if (context == null) EstablishDcBias();

            while (timestep > 0)
            {
                context.TimePoint = context.TimePoint + timestep;
                context.TimeStep = timestep;
                EstablishDcBias_Internal();
                OnDcBiasEstablished();
                timestep -= timestep;
            }
        }

        /// <summary>Establishes initial operating point for the transient analysis.</summary>
        public void EstablishDcBias(bool initCond = false)
        {
            context = null; // reset;
            EnsureInitialized();

            // initial condition
            for (int i = 0; i < initialVoltages.Length; i++)
            {
                if (initialVoltages[i].HasValue)
                {
                    initVoltProxies[2 * i].Add(1);
                    initVoltProxies[2 * i + 1].Add(initialVoltages[i].Value);
                }
            }

            if (!EstablishDcBias_Internal())
                throw new NonConvergenceException();

            // rerun without initial voltages
            if (!initCond && !EstablishDcBias_Internal())
                throw new NonConvergenceException();

            OnDcBiasEstablished();
        }

        private void EnsureInitialized()
        {
            if (context != null) return;
            context = new SimulationContext(CircuitParameters);

            // build equation system
            equationSystemAdapter = new EquationSystemAdapter(NodeCount);

            foreach (var device in Devices)
                device.Initialize(equationSystemAdapter, context);

            // get proxies for initial conditions
            initVoltProxies.Clear();
            for (int i = 0; i < this.initialVoltages.Length; i++)
            {
                if (initialVoltages[i].HasValue)
                {
                    initVoltProxies.Add(equationSystemAdapter.GetMatrixCoefficientProxy(i, i));
                    initVoltProxies.Add(equationSystemAdapter.GetRightHandSideCoefficientProxy(i));
                }
                else
                {
                    initVoltProxies.Add(null);
                    initVoltProxies.Add(null);
                }
            }
            // finalize making changes
            equationSystemAdapter.Freeze();

            // allocate temporary arrays
            currentSolution = new double[equationSystemAdapter.VariableCount];
            previousSolution = new double[equationSystemAdapter.VariableCount];
        }

        private bool EstablishDcBias_Internal()
        {
            LastNonLinearIterationCount = 0;
            LastNonLinearIterationDelta = 0;

            UpdateEquationSystem();
            SolveAndUpdateVoltages();

            double delta;

            do
            {
                delta = 0;

                UpdateEquationSystem();
                SolveAndUpdateVoltages();

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
            equationSystemAdapter.Anullate(0);

            var tmp = currentSolution;
            currentSolution = previousSolution;
            previousSolution = tmp;
            equationSystemAdapter.Solve(currentSolution);

            for (int i = 0; i < devices.Length; i++)
            {
                devices[i].OnEquationSolution(context);
            }

            for (var i = 0; i < NodeCount; i++)
            {
                NodeVoltages[i] = currentSolution[i];
            }
        }

        private void UpdateEquationSystem()
        {
            equationSystemAdapter.Clear();

            for (int i = 0; i < devices.Length; i++)
            {
                devices[i].ApplyModelValues(context);
            }
        }

        private class SimulationContext : ISimulationContext
        {
            public SimulationContext(CircuitParameters parameters)
            {
                CircuitParameters = parameters;
            }

            public double TimePoint { get; set; }
            public double TimeStep { get; set; }

            public CircuitParameters CircuitParameters { get; }
        }
    }
}