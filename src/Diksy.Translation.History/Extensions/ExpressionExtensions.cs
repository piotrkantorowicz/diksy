using System.Linq.Expressions;

namespace Diksy.Translation.History.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            InvocationExpression invokedExpr = Expression.Invoke(right, left.Parameters);

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(left.Body, invokedExpr), left.Parameters);
        }
    }
}