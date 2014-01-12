namespace Caliburn.Micro
{
    using System;

    public interface ILog
    {
        void Error(Exception exception);
        void Info(string format, params object[] args);
        void Warn(string format, params object[] args);
    }
}

