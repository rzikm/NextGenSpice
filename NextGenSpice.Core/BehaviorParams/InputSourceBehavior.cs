using System;

namespace NextGenSpice.Core.BehaviorParams
{
    /// <summary>Base class for classes that are used to specify behavior of input sources</summary>
    public abstract class InputSourceBehavior : ICloneable
    {
        /// <summary>Creates a new object that is a copy of the current instance.</summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}