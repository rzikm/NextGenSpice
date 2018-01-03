using System.IO;

namespace NextGenSpice
{
    public abstract class PrintStatement
    {
        public abstract void Initialize(object model);
        public abstract void PrintHeader(TextWriter output);
        public abstract void PrintValue(TextWriter output);
    }
}