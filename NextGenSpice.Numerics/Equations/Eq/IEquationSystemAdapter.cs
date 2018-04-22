using System;
using System.Collections.Generic;

namespace NextGenSpice.Numerics.Equations.Eq
{
    /// <summary>Defines methods for creating bindings on the equation system.</summary>
    public interface IEquationSystemAdapter
    {
        /// <summary>Adds a new variable to the equation system and returns the index of the variable;</summary>
        /// <returns></returns>
        int AddNewVariable();

        /// <summary>Returns proxy class for coefficient at given coordinates in the equation matrix.</summary>
        /// <param name="row">Row coordinate.</param>
        /// <param name="column">Column coordinate.</param>
        /// <returns></returns>
        IEquationSystemCoefficientProxy GetMatrixCoefficientProxy(int row, int column);

        /// <summary>Returns proxy class for coefficient at given row in the right hand side vector.</summary>
        /// <param name="row">Row coordinate.</param>
        /// <returns></returns>
        IEquationSystemCoefficientProxy GetRightHandSideCoefficientProxy(int row);

        /// <summary>Returns proxy class for the i-th variable of the solution.</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IEquationSystemSolutionProxy GetSolutionProxy(int index);
    }

    /// <summary>Class used to build and</summary>
    internal class EquationSystemAdapter : IEquationSystemAdapter
    {
        private int varCount;
        private readonly Dictionary<(int, int), MatrixProxy> matrixProxies;
        private readonly Dictionary<int, RhsProxy> rhsProxies;
        private readonly Dictionary<int, SolutionProxy> solutionProxies;

#if dd_precision
        private DdEquationSystem system;
#elif qd_precision
        private QdEquationSystem system;
#else
        private EquationSystem system;
#endif

        public EquationSystemAdapter(int varCount)
        {
            this.varCount = varCount;
            matrixProxies = new Dictionary<(int, int), MatrixProxy>();
            rhsProxies = new Dictionary<int, RhsProxy>();
            solutionProxies = new Dictionary<int, SolutionProxy>();
        }


        /// <summary>Adds a new variable to the equation system and returns the index of the variable;</summary>
        /// <returns></returns>
        public int AddNewVariable()
        {
            return ++varCount;
        }

        /// <summary>Returns proxy class for coefficient at given coordinates in the equation matrix.</summary>
        /// <param name="row">Row coordinate.</param>
        /// <param name="column">Column coordinate.</param>
        /// <returns></returns>
        public IEquationSystemCoefficientProxy GetMatrixCoefficientProxy(int row, int column)
        {
            if (row < 0 || row >= varCount) throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column >= varCount) throw new ArgumentOutOfRangeException(nameof(column));
            if (system != null) throw new InvalidOperationException("Equation system already frozen.");

            if (!matrixProxies.TryGetValue((row, column), out var proxy))
                proxy = matrixProxies[(row, column)] = new MatrixProxy(row, column);
            return proxy;
        }

        /// <summary>Returns proxy class for coefficient at given row in the right hand side vector.</summary>
        /// <param name="row">Row coordinate.</param>
        /// <returns></returns>
        public IEquationSystemCoefficientProxy GetRightHandSideCoefficientProxy(int row)
        {
            if (row < 0 || row >= varCount) throw new ArgumentOutOfRangeException(nameof(row));
            if (system != null) throw new InvalidOperationException("Equation system already frozen.");

            if (!rhsProxies.TryGetValue(row, out var proxy))
                proxy = rhsProxies[row] = new RhsProxy(row);
            return proxy;
        }

        /// <summary>Returns proxy class for the i-th variable of the solution.</summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IEquationSystemSolutionProxy GetSolutionProxy(int index)
        {
            if (index < 0 || index >= varCount) throw new ArgumentOutOfRangeException(nameof(index));
            if (system != null) throw new InvalidOperationException("Equation system already frozen.");

            if (!solutionProxies.TryGetValue(index, out var proxy))
                proxy = solutionProxies[index] = new SolutionProxy(index);
            return proxy;
        }

        /// <summary>Freezes the representation of the equation matrix.</summary>
        public void Freeze()
        {
            if (system != null) throw new InvalidOperationException("Equation system already frozen.");
#if dd_precision
            system = new DdEquationSystem(varCount);
#elif qd_precision
    system = new QdEquationSystem(varCount);
#else
        system = new EquationSystem(varCount);
#endif
            // set system to all proxies
            foreach (var proxy in matrixProxies.Values)
                proxy.system = system;
            foreach (var proxy in rhsProxies.Values)
                proxy.system = system;
            foreach (var proxy in solutionProxies.Values)
                proxy.system = system;
        }

        /// <summary>Solves the equation matrix and stores the result in the provided array.</summary>
        /// <param name="target"></param>
        public void Solve(double[] target)
        {
            if (system == null) throw new InvalidOperationException("Equation system must be frozen before accessing.");
            if (target.Length != varCount) throw new ArgumentException("The target array is of different size.");
            system.Solve();
            for (var i = 0; i < target.Length; i++) target[i] = (double) system.Solution[i];
        }

        private class MatrixProxy : IEquationSystemCoefficientProxy
        {
            private readonly int row;
            private readonly int col;

#if dd_precision
            public DdEquationSystem system;
#elif qd_precision
        public QdEquationSystem system;
#else
        public EquationSystem system;
#endif

            public MatrixProxy(int row, int col)
            {
                this.row = row;
                this.col = col;
            }

            public void Add(double value)
            {
                if (double.IsNaN(value)) throw new InvalidOperationException("Cannot insert NaN");
            }
        }

        private class RhsProxy : IEquationSystemCoefficientProxy
        {
            private readonly int row;

#if dd_precision
            public DdEquationSystem system;
#elif qd_precision
        public QdEquationSystem system;
#else
        public EquationSystem system;
#endif

            public RhsProxy(int row)
            {
                this.row = row;
            }

            public void Add(double value)
            {
                if (double.IsNaN(value)) throw new InvalidOperationException("Cannot insert NaN");
                system.RightHandSide[row] += value;
            }
        }

        private class SolutionProxy : IEquationSystemSolutionProxy
        {
            private readonly int row;

#if dd_precision
            public DdEquationSystem system;
#elif qd_precision
        public QdEquationSystem system;
#else
        public EquationSystem system;
#endif

            public SolutionProxy(int row)
            {
                this.row = row;
            }

            public double GetValue()
            {
                return (double) system.Solution[row];
            }
        }
    }

    /// <summary>Defines method for modifying a coefficient in the equation system representation.</summary>
    public interface IEquationSystemCoefficientProxy
    {
        /// <summary>Adds a specified value to the target coefficient.</summary>
        /// <param name="value"></param>
        /// <returns></returns>
        void Add(double value);
    }

    /// <summary>Defines method for reading the value of variable from equation system solution.</summary>
    public interface IEquationSystemSolutionProxy
    {
        /// <summary>Gets the value of the target solution variable.</summary>
        /// <returns></returns>
        double GetValue();
    }
}