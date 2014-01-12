namespace Caliburn.Micro
{
    using System.Collections;

    public interface IParent
    {
        IEnumerable GetChildren();
    }
}

