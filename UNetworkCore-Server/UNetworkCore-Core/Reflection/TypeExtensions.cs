using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace UNetworkCore_Core.Reflection
{
    public static class TypeExtensions
    {
        public static string GetIPString(this Socket socket)
        {
            return ((IPEndPoint)socket.RemoteEndPoint)?.Address?.ToString() ?? "Unknown";
        }
        public static bool IsSubclassOfGeneric(this Type type, Type genericType)
        {
            Type baseType = type.BaseType;
            bool result;
            while (baseType != null && !baseType.IsValueType)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == genericType)
                {
                    result = true;
                    return result;
                }
                baseType = baseType.BaseType;
            }
            result = false;
            return result;
        }
        public static bool HasAttribute(this MethodInfo method, Type type)
        {
            return method.CustomAttributes.Count(x => x.AttributeType == type) > 0;
        }
        public static CustomAttributeData GetAttribute(this MethodInfo method, int index = 0)
        {
            return method.CustomAttributes.ToArray()[index];
        }
        public static bool HasAttribute(this Type type, Type typeToFound)
        {
            return type.CustomAttributes.Count(x => x.AttributeType == typeToFound) > 0;
        }
        public static CustomAttributeData GetAttribute(this Type type, int index = 0)
        {
            return type.CustomAttributes.ToArray()[index];
        }
        public static CustomAttributeData GetAttribute(this object @object, int index = 0)
        {
            return @object.GetType().CustomAttributes.ToArray()[index];
        }
        public static T GetAttributeValue<T>(this CustomAttributeData attribute, int index = 0)
        {
            return (T)attribute.ConstructorArguments[index].Value;
        }
        public static object ConvertTo(this string value, Type conversionType)
        {
            return Convert.ChangeType(value, conversionType);
        }
    }
}
