﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Sillycore.EntityFramework.DynamicFiltering
{
    public class ExpressionMapper
    {
        public static IQueryable<T> GenerateProjectedQuery<T>(Type sourceType, Type targetType, IEnumerable<MemberBinding> binding, IQueryable source, ParameterExpression sourceParameter)
        {
            LambdaExpression typeSelector =
                Expression.Lambda(
                    Expression.MemberInit(
                        Expression.New(
                            targetType.GetConstructor(Type.EmptyTypes)
                        ),
                        binding
                    ),
                    sourceParameter
                );

            MethodCallExpression typeSelectExpression =
                Expression.Call(
                    typeof(Queryable),
                    "Select",
                    new[] { sourceType, targetType },
                    Expression.Constant(source),
                    typeSelector
                );

            return Expression.Lambda(typeSelectExpression)
                .Compile()
                .DynamicInvoke() as IQueryable<T>;
        }
    }
}
