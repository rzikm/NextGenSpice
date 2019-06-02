using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using BenchmarkDotNet.Code;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using NextGenSpice.LargeSignal;
using NextGenSpice.Numerics.Equations;
using NextGenSpice.Parser;

namespace SandboxRunner
{
    [CoreJob]
    public class PrecisionBenchmarks
    {
        private const string path = "..\\..\\..\\..\\..\\SandboxRunner\\ProfileCircuits\\";
        private const string suffix = ".sp";

        private class Config : ManualConfig
        {

        }


        private LargeSignalCircuitModel model;

        [ParamsSource(nameof(GetCircuits))] public SimulationRequest simulation;

        private int iterations;

        public IEnumerable<IParam> GetCircuits()
        {
            return new List<SimulationRequest>
            {
//                new SimulationRequest("adder", 1e-9, 50e-9),
//                new SimulationRequest("astable", 0.1e-6, 10e-6),
//                new SimulationRequest("backtoback", 100e-6, 10e-3),
//                new SimulationRequest("cfflop", 1e-9, 1000e-9),
                new SimulationRequest("choke", 0.2e-3, 20e-3),
//                new SimulationRequest("diffpair", 5e-9, 500e-9),
//                new SimulationRequest("ecl", 0.2e-9, 10e-9),
//                new SimulationRequest("rca3040", 0.5e-9, 200e-9),
//                new SimulationRequest("rtlinv", 2e-9, 200e-9),
//                new SimulationRequest("sbdgate", 1e-9, 200e-9),
//                new SimulationRequest("ua709", 2e-6, 250e-6),
            }.Select(i => new CustomParam(i));
        }

        private void LoadCircuit()
        {
            var result = SpiceNetlistParser.WithDefaults().Parse(new StreamReader(path + simulation.circuit + suffix));
            model = result.CircuitDefinition.GetLargeSignalModel();
        }

        [GlobalSetup(Target = nameof(Double))]
        public void SetDouble()
        {
            EquationSystemAdapterFactory.SetFactory(() => new EquationSystemAdapter());
            LoadCircuit();
        }

        [GlobalSetup(Target = nameof(DoubleDouble))]
        public void SetDoubleDouble()
        {
            EquationSystemAdapterFactory.SetFactory(() => new DdEquationSystemAdapter());
            LoadCircuit();
        }

        [GlobalSetup(Target = nameof(QuadDouble))]
        public void SetQuadDouble()
        {
            EquationSystemAdapterFactory.SetFactory(() => new QdEquationSystemAdapter());
            LoadCircuit();
        }

        private void Simulate()
        {
            iterations = 0;
            model.EstablishDcBias();
            iterations += model.LastNonLinearIterationCount;
            var timestep = simulation.timestep;
            var time = simulation.duration;
            while (time > 0)
            {
                model.AdvanceInTime(timestep);
                iterations += model.LastNonLinearIterationCount;
                time -= timestep;
            }
        }

        [Benchmark(Description = "double", Baseline = true)]
        public double Double()
        {
            Simulate();
            return 1;
        }

        [Benchmark(Description = "double-double")]
        public double DoubleDouble()
        {
            Simulate();
            return 1;
        }

        [Benchmark(Description = "quad-double")]
        public double QuadDouble()
        {
            Simulate();
            return 1;
        }

        public struct SimulationRequest
        {
            /// <summary>Initializes a new instance of the <see cref="T:System.Object"></see> class.</summary>
            public SimulationRequest(string circuit, double timestep, double duration)
            {
                this.circuit = circuit;
                this.timestep = timestep;
                this.duration = duration;
            }

            public string circuit { get; set; }
            public double timestep { get; set; }
            public double duration { get; set; }
        }

        public class CustomParam : IParam
        {
            private readonly SimulationRequest request;

            public CustomParam(SimulationRequest request)
            {
                this.request = request;
            }

            public string ToSourceCode()
            {
                return $"new SimulationRequest(\"{request.circuit}\", {request.timestep}, {request.duration})";
            }

            public object Value => request;

            public string DisplayText => request.circuit;
        }
    }
}