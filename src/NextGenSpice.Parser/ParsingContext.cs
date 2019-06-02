using System.Collections.Generic;
using System.Linq;
using NextGenSpice.Core.Circuit;
using NextGenSpice.Parser.Statements;
using NextGenSpice.Parser.Statements.Deferring;
using NextGenSpice.Parser.Utils;

namespace NextGenSpice.Parser
{
	public class ParsingScope
	{
		private readonly ParsingScope ParentScope;

		public ParsingScope(Token[] subcktStatement, ParsingScope parent = null)
		{
			SubcktStatement = subcktStatement;
			ParentScope = parent;
			CircuitBuilder = new CircuitBuilder();
			Statements = new List<DeferredStatement>();
			SymbolTable = new SymbolTable(parent?.SymbolTable);
			Errors = new List<SpiceParserError>();
			OtherStatements = new List<SpiceStatement>();
			Depth = parent?.Depth + 1 ?? 0;
		}

		public Token[] SubcktStatement { get; }

		public int Depth { get; }

		public CircuitBuilder CircuitBuilder { get; }
		public List<DeferredStatement> Statements { get; }
		public SymbolTable SymbolTable { get; }
		public List<SpiceParserError> Errors { get; }
		public List<SpiceStatement> OtherStatements { get; }
	}

	/// <summary>Class that hold intermediate data during paring of an input file.</summary>
	public class ParsingContext
	{
		private readonly List<ParsingScope> allScopes;
		private readonly Stack<ParsingScope> stack;

		public ParsingContext()
		{
			allScopes = new List<ParsingScope>();
			stack = new Stack<ParsingScope>();
			stack.Push(new ParsingScope(null));
			allScopes.Add(CurrentScope);
		}

		public ParsingScope CurrentScope => stack.Peek();

		public string Title { get; set; }

		/// <summary>Table containing known symbols from input file.</summary>
		public ISymbolTable SymbolTable => CurrentScope.SymbolTable;

		/// <summary>Set of errors from the input file.</summary>
		public List<SpiceParserError> Errors => CurrentScope.Errors;

		/// <summary>Set of all syntactically correct staements encountered to be evaluated.</summary>
		public List<DeferredStatement> DeferredStatements => CurrentScope.Statements;

		/// <summary>Statements that are recognized, but otherwise unused.</summary>
		public List<SpiceStatement> OtherStatements => CurrentScope.OtherStatements;

		/// <summary>Temporarily suspends parsing of current circuit and creates new frame to parse a subcircuit.</summary>
		public void EnterSubcircuit(Token[] stmt)
		{
			stack.Push(new ParsingScope(stmt, CurrentScope));
			allScopes.Add(CurrentScope);
		}

		/// <summary>Restores previous parsing frame.</summary>
		public void ExitSubcircuit()
		{
			stack.Pop();
		}

		/// <summary>Processes all deferred statements and if they cannot be processed, generate corresponding errors.</summary>
		public void FlushStatements()
		{
			// repeatedly try to process all statements until no more statements can be processed in the iteration
			// these repetitions are there to handle yet unknown statements with dependencies on later statements


			// first run: top down
			FlushOnce(allScopes);

			// second: bottom up
			allScopes.Sort((s1, s2) => -s1.Depth.CompareTo(s2.Depth));
			FlushRepeatedly(allScopes);
		}

		private void FlushRepeatedly(List<ParsingScope> scopes)
		{
			var stmts = scopes.SelectMany(s => s.Statements).ToList();
			var deferred = new List<DeferredStatement>();

			do
			{
				deferred.Clear();
				deferred.AddRange(stmts);
				stmts.Clear();
				foreach (var s in deferred)
					if (s.CanApply())
						s.Apply();
					else
						stmts.Add(s);
			} while (deferred.Count > stmts.Count);

			//            foreach (var scope in scopes)
			//            {
			//                var deferred = scope.Statements.ToList();
			//
			//                do
			//                {
			//                    scope.Statements.Clear();
			//                    scope.Statements.AddRange(deferred);
			//                    deferred.Clear();
			//
			//                    foreach (var statement in scope.Statements)
			//                        if (statement.CanApply())
			//                            statement.Apply();
			//                        else
			//                            deferred.Add(statement);
			//
			//                } while (deferred.Count < scope.Statements.Count);
			//
			//                // no more statements can be processed
			//                scope.Errors.AddRange(deferred.SelectMany(d => d.GetErrors()));
			//            }

			var errors = allScopes.SelectMany(s => s.Errors).Concat(stmts.SelectMany(s => s.GetErrors())).ToList();
			CurrentScope.Errors.Clear();
			CurrentScope.Errors.AddRange(errors);
		}

		private void FlushOnce(List<ParsingScope> scopes)
		{
			var stmts = scopes.SelectMany(s => s.Statements).ToList();
			foreach (var s in stmts)
				if (s.CanApply())
					s.Apply();
		}
	}
}