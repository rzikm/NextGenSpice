using System;
using System.Runtime.InteropServices.ComTypes;

namespace Numerics
{
    public unsafe struct Quad
    {
        private fixed double data[4];


        public override string ToString()
        {
            return "I am a Quad!";
        }
    }
}
