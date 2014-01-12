namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;

    public interface ICloseStrategy<T>
    {
        void Execute(IEnumerable<T> toClose, System.Action<bool, IEnumerable<T>> callback);
    }
}

