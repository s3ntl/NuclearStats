using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NS
{
    public static class ReflectionUtils
    {
        public static T ReadPrivateField<T>(string fieldName, object obj)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                Plugin.DebugLog($"Field {fieldName} not found", "ReflectionUtils");
                throw new InvalidOperationException($"Field {fieldName} not found");

            }
            return (T)fieldInfo.GetValue(obj);
        }

        public static void WritePrivateField<T>(string fieldName, T value, object obj)
        {
            FieldInfo fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (fieldInfo == null)
            {
                Plugin.DebugLog($"Field {fieldName} not found", "ReflectionUtils");
                throw new InvalidOperationException($"Field {fieldName} not found");
            }
            fieldInfo.SetValue(obj, value);
        }

        
        public static Delegate CreatePrivateMethodDelegate(Type delegateType, string methodName, object obj)
        {
            MethodInfo methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (methodInfo == null)
            {
                Plugin.DebugLog($"Method {methodName} not found", "ReflectionUtils");
                throw new InvalidOperationException($"Method {methodName} not found");
            }
            return Delegate.CreateDelegate(delegateType, obj, methodInfo);
        }
    }
}
