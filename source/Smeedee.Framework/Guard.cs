using System;
using System.Linq;

namespace Smeedee.Framework
{
    public static class Guard
    {
		public static void Requires<T>(bool expression) where T : Exception
		{
			if (expression == false)
				throw Activator.CreateInstance<T>();
		}

        public static void Requires<T>(bool expression, string message) where T : Exception
        {
            if (expression == false)
                throw Activator.CreateInstance(typeof (T), new object[] {message}) as T;
        }

		public static void ThrowExceptionIfNull(Object objectToTest, string parameterName)
		{
			if (objectToTest == null)
				throw new ArgumentException("The " + parameterName + " argument cannot be null");
		}

        public static void ThrowIfNull<T>(params object[] objects) where T : Exception
        {
            if (objects.Any(o => o == null))
                throw Activator.CreateInstance<T>();
        }

        public static ExceptionInfo Throw<T>() where T:Exception
        {
            return new ExceptionInfo(typeof(T));
        }

        public static void If(this ExceptionInfo subject, bool failurePredicate)
        {
            if( failurePredicate )
                throw Activator.CreateInstance(subject.ExceptionType) as Exception;
        }

        public class ExceptionInfo
        {
            internal Type ExceptionType { get; private set; }

            internal ExceptionInfo(Type exceptionType)
            {
                ExceptionType = exceptionType;
            }
        } 

    }
}
