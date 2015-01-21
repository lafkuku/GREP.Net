using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Grep.Net.Model
{
    public class ReflectionUtilities
    {
        public static bool TryGetPropertyValue(object target, string name, out object result)
        {
            var members = target.GetType().GetMember(name, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            if (members.Count() < 1)
            {
                result = null;
                return false;
            }

            MemberInfo member = members[0];

            if (member == null)
            {
                result = null;
                return false;
            }

            var prop = member as PropertyInfo;
            if (prop != null)
            {
                result = prop.GetValue(target, null);
                return true;
            }

            var method = member as MethodInfo;
            if (method != null)
            {
                var parameterTypes = (from p in method.GetParameters()
                                      select p.ParameterType).ToArray();
                var delegateType = method.ReturnType != typeof(void)
                                   ? Expression.GetFuncType(parameterTypes.Union(new[] { method.ReturnType }).ToArray())
                                   : Expression.GetActionType(parameterTypes);
                result = Delegate.CreateDelegate(delegateType, target, method.Name, true, false);
                return true;
            }
            result = null;
            return false;
        }

        public static bool InheritsFrom(object o, Type t)
        {
            Type tmpType = o.GetType();
            return InheritsFrom(tmpType, t);
        }

        public static bool InheritsFrom(Type item, Type inheritsFrom)
        {
            while (item != null)
            {
                if (item == inheritsFrom)
                {
                    return true;
                }
                item = item.BaseType;
            }
            return false;
        }

        public static bool IsIListOfGenericType(Type typeToCheck, Type genericType)
        {
            return typeToCheck.IsGenericType &&
                   typeToCheck.GetInterfaces().Contains(typeof(System.Collections.IList)) &&
                   typeToCheck.GetGenericArguments().Length > 0 &&
                   InheritsFrom(typeToCheck.GetGenericArguments()[0], genericType);
        }
    }
}