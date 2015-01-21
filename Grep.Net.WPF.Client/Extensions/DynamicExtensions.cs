using System;
using System.Linq;

namespace Grep.Net.WPF.Client.Extensions
{
    /*
    internal class DynamicInstanceInvokeWrapper : DynamicObject
    {
    private readonly IDictionary<String, MemberInfo> members = new Dictionary<string, MemberInfo>();
    private readonly Type type;
    private object target; 
    public DynamicInstanceInvokeWrapper(object target)
    {
    this.type = target.GetType();
    this.target = target; 
    var members = type.GetMembers(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
    foreach(MemberInfo member in members){
    this.members[member.Name] = member;
    }
    }
    public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
    {
    var name = indexes[0] as string;
    MemberInfo member;
    if (false == members.TryGetValue(name, out member))
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
    }
    */
}