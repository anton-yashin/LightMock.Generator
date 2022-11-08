using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LightMock
{
    /// <summary>
    /// An <see cref="ExpressionVisitor"/> that replaces references to the 
    /// <see cref="The{TValue}.IsAnyValue"/> with a <see cref="MethodCallExpression"/> 
    /// that represents calling the <see cref="The{TValue}.Is"/> method.    
    /// </summary>
    public class MatchExpressionRewriter : ExpressionVisitor
    {
        private MatchExpressionRewriter() { }
        /// <summary>
        /// Replaces references to the <see cref="The{TValue}.IsAnyValue"/> with a <see cref="MethodCallExpression"/>
        /// that represents calling the <see cref="The{TValue}.Is"/> method.
        /// </summary>
        /// <param name="expression">The <see cref="LambdaExpression"/> to visit.</param>
        /// <returns><see cref="Expression"/>.</returns>
        public static LambdaExpression Rewrite(LambdaExpression expression)
        {
            var @this = new MatchExpressionRewriter();
            return (LambdaExpression)@this.Visit(expression);
        }

        /// <summary>
        /// Replaces references to the <see cref="The{TValue}.IsAnyValue"/> with a <see cref="MethodCallExpression"/>
        /// that represents calling the <see cref="The{TValue}.Is"/> method.
        /// </summary>
        /// <param name="node">The <see cref="MemberExpression"/> to visit.</param>
        /// <returns><see cref="Expression"/>.</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            MemberInfo member = node.Member;

            switch (member.Name)
            {
                case nameof(The<object>.IsAnyValue)
                when member.DeclaringType.GetGenericTypeDefinition() == typeof(The<>):
                    return Create_Is_MethodCallExpression(member);
                case nameof(TheReference<object>.Value)
                when node.Expression is MemberExpression mex
                && mex.Member.Name == nameof(The<object>.Reference.IsAny)
                && mex.Member.DeclaringType.GetGenericTypeDefinition() == typeof(The<>.Reference):
                    return Create_The_Is_MethodCallExpression(mex);
            }
            return base.VisitMember(node);
        }

        private Expression Create_The_Is_MethodCallExpression(MemberExpression node)
            => Expression.Call(GetTheIsMethod(node.Member), TrueLambda(node.Member));

        private static Expression Create_Is_MethodCallExpression(MemberInfo member)
            => Expression.Call(GetIsMethod(member), TrueLambda(member));

        static Expression TrueLambda(MemberInfo member)
            => Expression.Lambda(
                body: Expression.Constant(true, typeof(bool)),
                Expression.Parameter(member.DeclaringType.GenericTypeArguments[0], "v")
                );

        private static MethodInfo GetIsMethod(MemberInfo member)
            => member.DeclaringType.GetTypeInfo().DeclaredMethods.Single(m => m.Name == nameof(The<object>.Is));

        static MethodInfo GetTheIsMethod(MemberInfo member)
            => typeof(The<>).MakeGenericType(member.DeclaringType.GetGenericArguments()).GetTypeInfo().DeclaredMethods.Single(m => m.Name == nameof(The<object>.Is));
    }
}