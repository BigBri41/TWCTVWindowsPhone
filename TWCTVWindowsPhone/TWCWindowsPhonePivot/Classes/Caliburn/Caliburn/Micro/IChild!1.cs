namespace Caliburn.Micro
{
    using System;

    public interface IChild<TParent> : IChild
    {
        TParent Parent { get; set; }
    }
}

