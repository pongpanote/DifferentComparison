using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace Assignment.RestServiceTest
{
    public static class TestHelpers
    {
        public static string GetBodyAsString(this HttpWebResponse response)
        {
            var bodyStream = response.GetResponseStream();
            var reader = new StreamReader(bodyStream);
            var body = reader.ReadToEnd();
            return body;
        }

        public static object RunStaticMethod(Type type, string methodName, params object[] parameterValues)
        {
            const BindingFlags FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            return RunMethod(type, methodName, null, parameterValues, FLAGS);
        }

        public static object RunInstanceMethod(object instance, string methodName, params object[] parameterValues)
        {
            const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            return RunMethod(instance.GetType(), methodName, instance, parameterValues, FLAGS);
        }

        private static object RunMethod(Type type, string methodName, object instance, object[] parameterValues, BindingFlags flags)
        {
            var methodInfo = type.GetMethod(methodName, flags);

            if (methodInfo == null)
            {
                throw new ArgumentException("There is no method '" + methodName + "' for type '" + type + "'.");
            }

            return methodInfo.Invoke(instance, parameterValues);
        }

        public static object GetMemberValue(object instance, string fieldName)
        {
            return GetMemberValue(instance, instance.GetType(), fieldName);
        }

        public static object GetStaticMemberValue(Type type, string fieldName)
        {
            return GetMemberValue(null, type, fieldName);
        }

        private static object GetMemberValue(object instance, Type type, string fieldName)
        {
            var field = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if (field == null)
            {
                throw new NotSupportedException("The object " + type.Name + " does not contain the field " + fieldName);
            }

            return field.GetValue(instance);
        }
    }
}
