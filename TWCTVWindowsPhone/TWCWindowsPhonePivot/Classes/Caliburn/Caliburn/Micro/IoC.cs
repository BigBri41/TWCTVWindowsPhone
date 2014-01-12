namespace Caliburn.Micro
{
    using System;

    public static class IoC
    {
        public static Action<object> BuildUp;
        public static System.Func<Type, IEnumerable<object>> GetAllInstances;
        public static System.Func<Type, string, object> GetInstance;

        public static T Get<T>()
        {
            return (T) GetInstance(typeof(T), null);
        }

        public static T Get<T>(string key)
        {
            return (T) GetInstance(typeof(T), key);
        }
    }
}

