//#undef dd_precision
//#undef qd_precision

using System;
using System.Collections.Generic;
using NextGenSpice.Numerics.Precision;


namespace NextGenSpice.Numerics.Equations
{
    /// <summary>Class used to build and</summary>
    public class EquationSystemAdapter : IEquationSystemAdapter
    {
        /// <summary>Number of variables in the equation system;</summary>
        public int VariableCount { get; private set; }

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

        public EquationSystemAdapter(int variableCount)
        {
            VariableCount = variableCount;
            matrixProxies = new Dictionary<(int, int), MatrixProxy>();
            rhsProxies = new Dictionary<int, RhsProxy>();
            solutionProxies = new Dictionary<int, SolutionProxy>();
        }


        /// <summary>Adds a new variable to the equation system and returns the index of the variable;</summary>
        /// <returns></returns>
        public int AddVariable()
        {
            return VariableCount++;
        }

        /// <summary>Returns proxy class for coefficient at given coordinates in the equation matrix.</summary>
        /// <param name="row">Row coordinate.</param>
        /// <param name="column">Column coordinate.</param>
        /// <returns></returns>
        public IEquationSystemCoefficientProxy GetMatrixCoefficientProxy(int row, int column)
        {
            if (row < 0 || row >= VariableCount) throw new ArgumentOutOfRangeException(nameof(row));
            if (column < 0 || column >= VariableCount) throw new ArgumentOutOfRangeException(nameof(column));
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
            if (row < 0 || row >= VariableCount) throw new ArgumentOutOfRangeException(nameof(row));
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
            if (index < 0 || index >= VariableCount) throw new ArgumentOutOfRangeException(nameof(index));
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
            system = new DdEquationSystem(VariableCount);
#elif qd_precision
    system = new QdEquationSystem(VariableCount);
#else
        system = new EquationSystem(VariableCount);
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
            if (target.Length != VariableCount) throw new ArgumentException("The target array is of different size.");
            system.Solve();
            for (var i = 0; i < target.Length; i++)
            {
                target[i] = (double)system.Solution[i];
            }
        }

        /// <summary>
        /// Enforces value 0 of a particular eqation system variable.
        /// </summary>
        /// <param name="index"></param>
        public void Anullate(int index)
        {
            var m = system.Matrix;

#if dd_precision
            for (int i = 0; i < m.Size; i++)
            {
                m[i, index] = dd_real.Zero;
                m[index, i] = dd_real.Zero;
            }

            m[index, index] = new dd_real(1);
            system.RightHandSide[index] = dd_real.Zero;
#elif qd_precision
            for (var i = 0; i < m.Size; i++)
            {
                m[i, index] = qd_real.Zero;
                m[index, i] = qd_real.Zero;
            }

            m[index, index] = new qd_real(1);
            system.RightHandSide[index] = qd_real.Zero;
#else
            for (int i = 0; i < m.Size; i++)
            {
                m[i, index] = 0;
                m[index, i] = 0;
            }

            m[index, index] = 1;
            system.RightHandSide[index] = 0;
#endif
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
                if (double.IsNaN(value)) throw new ArgumentNaNException("Cannot insert NaN");
                system.Matrix[row, col] += value;
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
                if (double.IsNaN(value)) throw new ArgumentNaNException("Cannot insert NaN");
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
                return (double)system.Solution[row];
            }
        }

        public void Clear()
        {
            for (int i = 0; i < system.Matrix.RawData.Length; ++i)
            {

#if dd_precision
                system.Matrix.RawData[i] = new dd_real(0);
#elif qd_precision
        system.Matrix.RawData[i] = new qd_real(0); 
#else
        system.Matrix.RawData[i] = 0;
#endif
            }

            for (int i = 0; i < system.RightHandSide.Length; ++i)
            {

#if dd_precision
                system.RightHandSide[i] = new dd_real(0);
#elif qd_precision
        system.RightHandSide[i] = new qd_real(0); 
#else
        system.RightHandSide[i] = 0;
#endif
            }
        }
    }
}