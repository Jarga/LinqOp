using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using System.Reflection;

// https://github.com/ImaginaryDevelopment/LinqOp
// https://www.nuget.org/packages/LinqOp/

namespace ImaginaryDevelopment.Helpers
{
        /// http://codebetter.com/patricksmacchia/2010/06/28/elegant-infoof-operators-in-c-read-info-of/
        public static class LinqOp
        {
            public static MethodInfo MethodOf<T>(Expression<Func<T>> expression)
            {
                var body = (MethodCallExpression)expression.Body;

                return body.Method;
            }

            public static MethodInfo MethodOf(Expression<Action> expression)
            {
                var body = (MethodCallExpression)expression.Body;

                return body.Method;
            }

            public static MethodInfo MethodOf<T>(Expression<Func<T, object>> expression)
            {
                var methExp = (MethodCallExpression)MaybeUnary(expression);

                return methExp.Method;
            }

            public static MethodInfo MethodOf<T>(Expression<Action<T>> expression)
            {

                var methExp = (MethodCallExpression)MaybeUnary(expression);

                return methExp.Method;
            }

            public static ConstructorInfo ConstructorOf<T>(Expression<Func<T>> expression)
            {
                var body = (NewExpression)expression.Body;

                return body.Constructor;
            }

            /// <summary>
            /// For getting a static property name, for instance properties use the other overload   
            /// Usage Example: LinqOp.PropertyOf(() => request.Foo).Name)
            /// </summary>
            public static PropertyInfo PropertyOf<T>(Expression<Func<T>> expression)
            {
                var body = (MemberExpression)expression.Body;

                return (PropertyInfo)body.Member;
            }

            /// <summary>
            /// Usage existing.GetItemProperty(e => e.Bar).Name
            /// </summary>
            public static PropertyInfo GetItemProperty<T>(this IEnumerable<T> items, Expression<Func<T, object>> expr)
            {
                return PropertyOf<T>(expr);
            }

            /// <summary>
            /// Store a simple reference to your LinqOp delegate for reuse throughout the class (or any scope)
            /// </summary>
            /// <returns></returns>
            public static Func<Expression<Func<T, object>>, string> PropertyNameHelper<T>()
            {
                return e => PropertyOf(e).Name;
            }

            public static PropertyInfo PropertyOf<T>(Expression<Func<T, object>> expression)
            {
                var memExp = (MemberExpression)MaybeUnary(expression);


                return (PropertyInfo)memExp.Member;
            }

            public static FieldInfo FieldOf<T>(Expression<Func<T>> expression)
            {
                var body = (MemberExpression)expression.Body;

                return (FieldInfo)body.Member;
            }

            public static MethodInfo GetActionInfoOf<T>(Expression<Action<T>> expression)
            {
                MethodCallExpression body = (MethodCallExpression)expression.Body;
                return body.Method;
            }

            public class BindingPropertyResult
            {
                public string BindingName { get; set; }
                public string SimpleName { get; set; }

                public PropertyInfo MemberPropertyInfo { get; set; }
                public PropertyInfo BindingPropertyInfo { get; set; }

                public bool IsNested { get; set; }
            }

            public static BindingPropertyResult BindingPropertyOf<T>(Expression<Func<T>> expression)
            {
                MemberExpression body = expression.Body as MemberExpression;
                if (body == null)
                {
                    UnaryExpression ubody = (UnaryExpression)expression.Body;
                    body = (MemberExpression)ubody.Operand;
                }

                MemberExpression superExpression = (MemberExpression)body.Expression;
                string superName = superExpression.Member.Name;
                PropertyInfo superProp = superExpression.Member as PropertyInfo;

                return new BindingPropertyResult()
                {
                    BindingName = superName + "." + body.Member.Name,
                    BindingPropertyInfo = superProp,
                    IsNested = superProp != null,
                    MemberPropertyInfo = body.Member as PropertyInfo,
                    SimpleName = body.Member.Name
                };
            }

            static Expression MaybeUnary<T>(Expression<T> exp)
            {
                Expression result;
                var uExp = exp.Body as UnaryExpression;
                result = uExp != null ? uExp.Operand : exp.Body;
                return result;
            }
        }
    }
