namespace Caliburn.Micro
{
    using System.Collections.Generic;

    public interface IParent<T> : IParent
    {
        IEnumerable<T> GetChildren();
    }
}

