using System;
using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Exceptions;
using NextGenSpice.Core.Representation;
using NextGenSpice.LargeSignal.Models;
using NextGenSpice.Numerics.Equations;
#if qd_precision || dd_precision
using NextGenSpice.Numerics.Precision;
#endif

namespace NextGenSpice.LargeSignal
{
    /// <summary>Main class for performing large signal analysis on electrical circuits.</summary>
    public class LargeSignalCircuitModel : IAnalysisCircuitModel<ILargeSignalDeviceModel>
    {
        private SimulationContext context;

        private double[] previousSolution;

#if qd_precision
        private QdEquationSystem equationSystem;
#elif dd_precision
        private DdEquationSystem equationSystem;
#else
        private EquationSystem equationSystem;
#endif

        public LargeSignalCircuitModel(IEnumerable<double?> initialVoltages, List<ILargeSignalDeviceModel> devices)
        {
            this.initialVoltages = initialVoltages.ToArray();
            NodeVoltages = new double[this.initialVoltages.Length];
            this.devices = devices.ToArray();

            deviceLookup = devices.Where(e => !string.IsNullOrEmpty(e.Name)).ToDictionary(e => e.Name);
            updaterInitCondition = e => e.ApplyInitialCondition(equationSystem, context);
            updaterModelValues = e => e.ApplyModelValues(equationSystem, context);
        }

        /// <summary>Last computed node voltages.</summary>
        public double[] NodeVoltages { get; }

        /// <summary>Number of node in the circuit.</summary>
        public int NodeCount => NodeVoltages.Length;

        /// <summary>Set of all devices that this circuit model consists of.</summary>
        public IReadOnlyList<ILargeSignalDeviceModel> Devices => devices;

        private ILargeSignalDeviceModel[] constDevices;
        private ILargeSignalDeviceModel[] nonlinearDevices;
        private ILargeSignalDeviceModel[] linearTimeDependentDevices;

        private readonly Dictionary<string, ILargeSignalDeviceModel> deviceLookup;
        private readonly double?[] initialVoltages;
        private readonly ILargeSignalDeviceModel[] devices;

        // cached functors to prevent excessive allocations
        private readonly Action<ILargeSignalDeviceModel> updaterInitCondition;
        private readonly Action<ILargeSignalDeviceModel> updaterModelValues;

        /// <summary>Gets model for the device identified by given name.</summary>
        /// <param name="name">Name of the device.</param>
        /// <returns></returns>
        public ILargeSignalDeviceModel GetDevice(string name)
        {
            return deviceLookup[name];
        }

        /// <summary>Gets model for the device identified by given name.</summary>
        /// <param name="name">Name of the device.</param>
        /// <param name="value">The device model.</param>
        /// <returns></returns>
        public bool TryGetDevice(string name, out ILargeSignalDeviceModel value)
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
                var step = 2 * Math.Min(MaxTimeStep, timestep);

                var timePoint = context.TimePoint;
                //                do
                //                {
                step /= 2;
                context.TimePoint = timePoint + step;
                context.TimeStep = step;
                EstablishDcBias_Internal(updaterModelValues);

                //                } while (!EstablishDcBias_Internal(e => e.ApplyModelValues(equationSystem, context)));
                OnDcBiasEstablished();
                timestep -= step;
            }
        }

        /// <summary>Establishes initial operating point for the transient analysis.</summary>
        public void EstablishInitialDcBias(bool initCond = false)
        {
            context = null;
            EnsureInitialized();
            //            context.TimeStep = 0;
            //            context.TimePoint = 0;

            for (int i = 0; i < initialVoltages.Length; i++)
            {
                if (initialVoltages[i].HasValue) // initial condition
                {
                    context.EquationSystem
                        .AddCurrent(i, 0, initialVoltages[i].Value)
                        .AddConductance(i, 0, 1);
                }
            }

            if (!EstablishDcBias_Internal(updaterInitCondition))
                throw new NonConvergenceException();

            // rerun without initial voltages
            equationSystem.Restore(0);
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
            context.EquationSystem = equationSystem;
            previousSolution = new double[context.EquationSystem.VariablesCount];
        }

        private void BuildEquationSystem()
        {
#if qd_precision
            var b = new QdEquationSystemBuilder();
#elif dd_precision
            var b = new DdEquationSystemBuilder();
#else
            var b = new EquationSystemBuilder();
#endif
            for (var i = 0; i < NodeCount; i++)
                b.AddVariable();

            foreach (var device in Devices)
                device.Initialize(b, context);

            foreach (var device in constDevices)
                device.ApplyModelValues(b, context);

            equationSystem = b.Build();
        }

        private bool EstablishDcBias_Internal(Action<ILargeSignalDeviceModel> updater)
        {
            LastNonLinearIterationCount = 0;
            LastNonLinearIterationDelta = 0;


            UpdateEquationSystem(updater, linearTimeDependentDevices);
            equationSystem.Backup(1);
            UpdateEquationSystem(updater, nonlinearDevices);

            SolveAndUpdateVoltages();
            //            DebugPrint();
            return IsLinear || IterateUntilConvergence(updater);
        }

        private bool IterateUntilConvergence(Action<ILargeSignalDeviceModel> updater)
        {
            double delta;

            do
            {
                delta = 0;

                equationSystem.Solution.CopyTo(previousSolution, 0);

                equationSystem.Restore(1);

                UpdateEquationSystem(updater, nonlinearDevices);
                SolveAndUpdateVoltages();
                //            DebugPrint();

                for (var i = 0; i < previousSolution.Length; i++)
                {
                    var d = previousSolution[i] - equationSystem.Solution[i];
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
            equationSystem.Restore(0);
        }

        private void SolveAndUpdateVoltages()
        {
            // ensure ground has 0 voltage
            var m = equationSystem.Matrix;

#if qd_precision
            for (int i = 0; i < m.Size; i++)
            {
                m[i, 0] = qd_real.Zero;
                m[0, i] = qd_real.Zero;
            }

            m[0, 0] = new qd_real(1);
            equationSystem.RightHandSide[0] = qd_real.Zero;
#elif dd_precision
            for (var i = 0; i < m.Size; i++)
            {
                m[i, 0] = dd_real.Zero;
                m[0, i] = dd_real.Zero;
            }

            m[0, 0] = new dd_real(1);
            equationSystem.RightHandSide[0] = dd_real.Zero;
#else
            for (int i = 0; i < m.Size; i++)
            {
                m[i, 0] = 0;
                m[0, i] = 0;
            }

            m[0, 0] = 1;
            equationSystem.RightHandSide[0] = 0;
#endif

            equationSystem.Solve();

            for (var i = 0; i < NodeCount; i++)
                NodeVoltages[i] = equationSystem.Solution[i];
        }

        private void UpdateEquationSystem(Action<ILargeSignalDeviceModel> updater,
            ILargeSignalDeviceModel[] elem)
        {
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

            public double GetSolutionForVariable(int index)
            {
                return EquationSystem.Solution[index];
            }

            public CircuitParameters CircuitParameters { get; }

#if qd_precision
            public QdEquationSystem EquationSystem { get; set; }
#elif dd_precision
            public DdEquationSystem EquationSystem { get; set; }
#else
            public EquationSystem EquationSystem { get; set; }
#endif
        }
    }
}