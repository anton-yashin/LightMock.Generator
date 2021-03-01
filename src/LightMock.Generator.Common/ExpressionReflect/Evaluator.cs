/*****************************************************************************   
	The MIT License (MIT)

	Copyright (c) 2013 Matthias Gernand

	Permission is hereby granted, free of charge, to any person obtaining a copy of
	this software and associated documentation files (the "Software"), to deal in
	the Software without restriction, including without limitation the rights to
	use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
	the Software, and to permit persons to whom the Software is furnished to do so,
	subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
	FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
	COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
	IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
	CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
******************************************************************************    
	https://github.com/mgernand
******************************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ExpressionReflect
{
	/// <summary>
	/// Enables the partial evaluation of queries.
	/// </summary>
	/// <remarks>
	/// From http://msdn.microsoft.com/en-us/library/bb546158.aspx
	/// </remarks>
	[ExcludeFromCodeCoverage]
	public static class Evaluator
	{
		/// <summary>
		/// Performs evaluation & replacement of independent sub-trees
		/// </summary>
		/// <param name="expression">The root of the expression tree.</param>
		/// <returns>A new tree with sub-trees evaluated and replaced.</returns>
		public static Expression PartialEval(this Expression expression)
		{
			return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
		}

		/// <summary>
		/// Performs evaluation & replacement of independent sub-trees
		/// </summary>
		/// <param name="expression">The root of the expression tree.</param>
		/// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
		/// <returns>A new tree with sub-trees evaluated and replaced.</returns>
		public static Expression PartialEval(this Expression expression, Func<Expression, bool> fnCanBeEvaluated)
		{
			return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
		}

		private static bool CanBeEvaluatedLocally(Expression expression)
		{
			return expression.NodeType != ExpressionType.Parameter;
		}

		/// <summary>
		/// Evaluates & replaces sub-trees when first candidate is reached (top-down)
		/// </summary>
		private class SubtreeEvaluator : ExpressionVisitor
		{
			private readonly IDictionary<Expression, Expression> candidates;

			internal SubtreeEvaluator(IDictionary<Expression, Expression> candidates)
			{
				this.candidates = candidates;
			}

			internal Expression Eval(Expression exp)
			{
				return this.Visit(exp);
			}

			public override Expression Visit(Expression exp)
			{
				if (exp == null)
				{
					return null;
				}
				if (this.candidates.ContainsKey(exp))
				{
					return this.Evaluate(exp);
				}
				return base.Visit(exp);
			}

			private Expression Evaluate(Expression e)
			{
				if (e.NodeType == ExpressionType.Constant)
				{
					return e;
				}

				LambdaExpression lambda = Expression.Lambda(e);
				object value = lambda.Execute();
				return Expression.Constant(value, e.Type);
			}
		}

		/// <summary>
		/// Performs bottom-up analysis to determine which nodes can possibly
		/// be part of an evaluated sub-tree.
		/// </summary>
		private class Nominator : ExpressionVisitor
		{
			private readonly Func<Expression, bool> fnCanBeEvaluated;
			private IDictionary<Expression, Expression> candidates;
			private bool cannotBeEvaluated;

			internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
			{
				this.fnCanBeEvaluated = fnCanBeEvaluated;
			}

			internal IDictionary<Expression, Expression> Nominate(Expression expression)
			{
				this.candidates = new Dictionary<Expression, Expression>();
				this.Visit(expression);
				return this.candidates;
			}

			public override Expression Visit(Expression expression)
			{
				if (expression != null)
				{
					bool saveCannotBeEvaluated = this.cannotBeEvaluated;
					this.cannotBeEvaluated = false;
					base.Visit(expression);
					if (!this.cannotBeEvaluated)
					{
						if (this.fnCanBeEvaluated(expression) && !this.candidates.ContainsKey(expression))
						{
							this.candidates.Add(expression, expression);
						}
						else
						{
							this.cannotBeEvaluated = true;
						}
					}
					this.cannotBeEvaluated |= saveCannotBeEvaluated;
				}
				return expression;
			}
		}
	}
}