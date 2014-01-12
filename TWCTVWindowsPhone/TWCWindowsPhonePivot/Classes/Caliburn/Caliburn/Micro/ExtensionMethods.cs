namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ExtensionMethods
    {
        public static void Apply<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T local in enumerable)
            {
                action(local);
            }
        }

        public static string GetAssemblyName(this Assembly assembly)
        {
            return assembly.FullName.Remove(assembly.FullName.IndexOf(","));
        }

        public static IEnumerable<T> GetAttributes<T>(this MemberInfo member, bool inherit)
        {
            return Attribute.GetCustomAttributes(member, inherit).OfType<T>();
        }

        public static MemberInfo GetMemberInfo(this Expression expression)
        {
            MemberExpression operand;
            LambdaExpression expression2 = (LambdaExpression) expression;
            if (expression2.Body is UnaryExpression)
            {
                UnaryExpression body = (UnaryExpression) expression2.Body;
                operand = (MemberExpression) body.Operand;
            }
            else
            {
                operand = (MemberExpression) expression2.Body;
            }
            return operand.Member;
        }
    }
}

