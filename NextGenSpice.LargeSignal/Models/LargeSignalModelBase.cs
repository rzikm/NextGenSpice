﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Core.Elements;
using NextGenSpice.Core.Equations;

namespace NextGenSpice.LargeSignal.Models
{
    public abstract class LargeSignalModelBase<TDefinitionElement> : ILargeSignalDeviceModel where TDefinitionElement : ICircuitDefinitionElement
    {
        public string Tag { get; set; }

        protected LargeSignalModelBase(TDefinitionElement parent)
        {
            Parent = parent;
        }

        protected TDefinitionElement Parent { get; }
        
        public virtual void Initialize(IEquationSystemBuilder builder)
        {
        }

        public virtual void PostProcess(SimulationContext context)
        {
            
        }
    }
}