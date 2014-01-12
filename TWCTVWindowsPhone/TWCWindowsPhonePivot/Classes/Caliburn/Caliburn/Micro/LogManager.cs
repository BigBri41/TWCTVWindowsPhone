namespace Caliburn.Micro
{
    using System;
    using System.Runtime.CompilerServices;

    public static class LogManager
    {
        public static System.Func<Type, ILog> GetLog = type => NullLogInstance;
        private static readonly ILog NullLogInstance = new NullLog();

        [CompilerGenerated]
        private static ILog <.cctor>b__0(Type type)
        {
            return NullLogInstance;
        }

        private class NullLog : ILog
        {
            public void Error(Exception exception)
            {
            }

            public void Info(string format, params object[] args)
            {
            }

            public void Warn(string format, params object[] args)
            {
            }
        }
    }
}

