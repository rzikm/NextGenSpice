//#undef dd_precision
//#undef qd_precision

using System;
using System.Collections.Generic;

namespace NextGenSpice.Numerics.Equations
{
	/// <summary>
	///   Provides a place for getting an implementation of <see cref="IEquationSystemAdapterWide" /> interface. This
	///   exists only for the purpose of changing the precision method used during runtime for benchmarking purposes.
	/// </summary>
	public static class EquationSystemAdapterFactory
	{
		private static Func<IEquationSystemAdapterWide> factory;

		static EquationSystemAdapterFactory()
		{
#if dd_precision
			SetFactory(() => new DdEquationSystemAdapter());
#elif qd_precision
            SetFactory(() => new QdEquationSystemAdapter());
#else
            SetFactory(() => new EquationSystemAdapter());
#endif
		}

		/// <summary>Creates a new instance of <see cref="IEquationSystemAdapterWide" /> based on the current factory method.</summary>
		/// <returns></returns>
		public static IEquationSystemAdapterWide GetEquationSystemAdapter()
		{
			return factory();
		}

		/// <summary>Sets a new factory method for the <see cref="IEquationSystemAdapterWide" /> implementation.</summary>
		/// <param name="newFactory"></param>
		public static void SetFactory(Func<IEquationSystemAdapterWide> newFactory)
		{
			factory = newFactory;
		}
	}


	public interface IEquationSystemAdapterWide : IEquationSystemAdapter
	{
		/// <summary>Number of variables in the equation system;</summary>
		int VariableCount { get; }

		/// <summary>Freezes the representation of the equation matrix.</summary>
		void Freeze();

		/// <summary>Solves the equation matrix and stores the result in the provided array.</summary>
		/// <param name="target"></param>
		void Solve(double[] target);

		/// <summary>Enforces value 0 of a particular eqation system variable.</summary>
		/// <param name="index"></param>
		void Anullate(int index);

		void Clear();
	}

	/// <summary>Class providing equation system proxy objects for individual equation coefficients in double precision</summary>
	public class EquationSystemAdapter : IEquationSystemAdapterWide
	{
		private readonly Dictionary<(int, int), MatrixProxy> matrixProxies;
		private readonly Dictionary<int, RhsProxy> rhsProxies;
		private readonly Dictionary<int, SolutionProxy> solutionProxies;

		private EquationSystem system;

		public EquationSystemAdapter()
		{
			matrixProxies = new Dictionary<(int, int), MatrixProxy>();
			rhsProxies = new Dictionary<int, RhsProxy>();
			solutionProxies = new Dictionary<int, SolutionProxy>();
		}

		/// <summary>Number of variables in the equation system;</summary>
		public int VariableCount { get; private set; }


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

			system = new EquationSystem(VariableCount);
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
			for (var i = 0; i < target.Length; i++) target[i] = system.Solution[i];
		}

		/// <summary>Enforces value 0 of a particular eqation system variable.</summary>
		/// <param name="index"></param>
		public void Anullate(int index)
		{
			var m = system.Matrix;

			for (var i = 0; i < m.Size; i++)
			{
				m[i, index] = 0;
				m[index, i] = 0;
			}

			m[index, index] = 1;
			system.RightHandSide[index] = 0;
		}

		public void Clear()
		{
			for (var i = 0; i < system.Matrix.RawData.Length; ++i) system.Matrix.RawData[i] = 0;

			for (var i = 0; i < system.RightHandSide.Length; ++i) system.RightHandSide[i] = 0;
		}

		private class MatrixProxy : IEquationSystemCoefficientProxy
		{
			private readonly int col;
			private readonly int row;

			public EquationSystem system;

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

			public EquationSystem system;

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

			public EquationSystem system;

			public SolutionProxy(int row)
			{
				this.row = row;
			}

			public double GetValue()
			{
				return system.Solution[row];
			}
		}
	}

#if dd_precision
	/// <summary>Class providing equation system proxy objects for individual equaiton coefficients in double-double precision</summary>
	public class DdEquationSystemAdapter : IEquationSystemAdapterWide
	{
		private readonly Dictionary<(int, int), MatrixProxy> matrixProxies;
		private readonly Dictionary<int, RhsProxy> rhsProxies;
		private readonly Dictionary<int, SolutionProxy> solutionProxies;

		private DdEquationSystem system;

		public DdEquationSystemAdapter()
		{
			matrixProxies = new Dictionary<(int, int), MatrixProxy>();
			rhsProxies = new Dictionary<int, RhsProxy>();
			solutionProxies = new Dictionary<int, SolutionProxy>();
		}

		/// <summary>Number of variables in the equation system;</summary>
		public int VariableCount { get; private set; }


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

			system = new DdEquationSystem(VariableCount);
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
			for (var i = 0; i < target.Length; i++) target[i] = (double) system.Solution[i];
		}

		/// <summary>Enforces value 0 of a particular eqation system variable.</summary>
		/// <param name="index"></param>
		public void Anullate(int index)
		{
			var m = system.Matrix;

			for (var i = 0; i < m.Size; i++)
			{
				m[i, index] = 0;
				m[index, i] = 0;
			}

			m[index, index] = 1;
			system.RightHandSide[index] = 0;
		}

		public void Clear()
		{
			for (var i = 0; i < system.Matrix.RawData.Length; ++i) system.Matrix.RawData[i] = 0;

			for (var i = 0; i < system.RightHandSide.Length; ++i) system.RightHandSide[i] = 0;
		}

		private class MatrixProxy : IEquationSystemCoefficientProxy
		{
			private readonly int col;
			private readonly int row;

			public DdEquationSystem system;

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

			public DdEquationSystem system;

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

			public DdEquationSystem system;

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
#endif

#if qd_precision
	/// <summary>Class providing equation system proxy objects for individual equaiton coefficients in quad-double precision</summary>
	public class QdEquationSystemAdapter : IEquationSystemAdapterWide
	{
		private readonly Dictionary<(int, int), MatrixProxy> matrixProxies;
		private readonly Dictionary<int, RhsProxy> rhsProxies;
		private readonly Dictionary<int, SolutionProxy> solutionProxies;

		private QdEquationSystem system;

		public QdEquationSystemAdapter()
		{
			matrixProxies = new Dictionary<(int, int), MatrixProxy>();
			rhsProxies = new Dictionary<int, RhsProxy>();
			solutionProxies = new Dictionary<int, SolutionProxy>();
		}

		/// <summary>Number of variables in the equation system;</summary>
		public int VariableCount { get; private set; }


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

			system = new QdEquationSystem(VariableCount);
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
			for (var i = 0; i < target.Length; i++) target[i] = (double) system.Solution[i];
		}

		/// <summary>Enforces value 0 of a particular eqation system variable.</summary>
		/// <param name="index"></param>
		public void Anullate(int index)
		{
			var m = system.Matrix;

			for (var i = 0; i < m.Size; i++)
			{
				m[i, index] = 0;
				m[index, i] = 0;
			}

			m[index, index] = 1;
			system.RightHandSide[index] = 0;
		}

		public void Clear()
		{
			for (var i = 0; i < system.Matrix.RawData.Length; ++i) system.Matrix.RawData[i] = 0;

			for (var i = 0; i < system.RightHandSide.Length; ++i) system.RightHandSide[i] = 0;
		}

		private class MatrixProxy : IEquationSystemCoefficientProxy
		{
			private readonly int col;
			private readonly int row;

			public QdEquationSystem system;

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

			public QdEquationSystem system;

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

			public QdEquationSystem system;

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
#endif
}