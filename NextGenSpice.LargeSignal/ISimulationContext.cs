﻿using System.Collections.Generic;
using NextGenSpice.LargeSignal.Models;

namespace NextGenSpice.LargeSignal
{
    /// <summary>Defines properties for getting information about the simulation.</summary>
    public interface ISimulationContext
    {
        /// <summary>Curent timepoint of the simulation.</summary>
        double TimePoint { get; }

        /// <summary>Last timestep that was used to advance the timepoint.</summary>
        double TimeStep { get; }

        /// <summary>General parameters of the circuit that is simulated.</summary>
        CircuitParameters CircuitParameters { get; }
    }
}