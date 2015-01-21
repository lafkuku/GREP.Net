using System;
using System.Linq;
using System.Reflection;

namespace Grep.Net.Model.Extensions
{
    public static class ObjectExtensions
    {
        public static object GetPropertyValue(this Object fromObject, string propertyName)
        {
            Type objectType = fromObject.GetType();
            PropertyInfo propInfo = objectType.GetProperty(propertyName);
            if (propInfo == null && propertyName.Contains('.'))
            {
                string firstProp = propertyName.Substring(0, propertyName.IndexOf('.'));
                propInfo = objectType.GetProperty(firstProp);
                if (propInfo == null)//property name is invalid
                {
                    throw new ArgumentException(String.Format("Property {0} is not a valid property of {1}.", firstProp, fromObject.GetType().ToString()));
                }
                return GetPropertyValue(propInfo.GetValue(fromObject, null), propertyName.Substring(propertyName.IndexOf('.') + 1));
            }
            else
            {
                return propInfo.GetValue(fromObject, null);
            }
        }
    }
}