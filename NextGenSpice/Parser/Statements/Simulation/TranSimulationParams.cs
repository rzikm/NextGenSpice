namespace NextGenSpice.Parser.Statements.Simulation
{
    /// <summary>Defines set of parameters for .TRAN simulation statement</summary>
    public class TranSimulationParams
    {
        /// <summary>Time point from which to start saving data.</summary>
        public double StartTime { get; set; }

        /// <summary>Time point when the simulation should stop.</summary>
        public double StopTime { get; set; }

        /// <summary>Suggested time step for numerical integration. Simulator may reduce or increase this value as needed.</summary>
        public double TimeStep { get; set; }

        // this option is not yet supported
//        /// <summary>
//        /// Maximmum allowed time step for numerical integration.
//        /// </summary>
//        public double MaximmumTimestep { get; set; }
    }
}