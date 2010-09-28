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
    }
}
