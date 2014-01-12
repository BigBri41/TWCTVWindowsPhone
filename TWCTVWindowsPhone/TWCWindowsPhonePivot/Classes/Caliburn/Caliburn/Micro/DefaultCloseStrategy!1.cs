namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class DefaultCloseStrategy<T> : ICloseStrategy<T>
    {
        private List<T> closable;
        private readonly bool closeConductedItemsWhenConductorCannotClose;
        private bool finalResult;

        public DefaultCloseStrategy(bool closeConductedItemsWhenConductorCannotClose = false)
        {
            this.closeConductedItemsWhenConductorCannotClose = closeConductedItemsWhenConductorCannotClose;
        }

        private void Evaluate(bool result, IEnumerator<T> enumerator, System.Action<bool, IEnumerable<T>> callback)
        {
            this.finalResult = this.finalResult && result;
            if (!enumerator.MoveNext())
            {
                callback(this.finalResult, this.closeConductedItemsWhenConductorCannotClose ? this.closable : new List<T>());
                this.closable = null;
            }
            else
            {
                Action<bool> action = null;
                T current = enumerator.Current;
                IGuardClose close = current as IGuardClose;
                if (close != null)
                {
                    if (action == null)
                    {
                        action = delegate (bool canClose) {
                            if (canClose)
                            {
                                ((DefaultCloseStrategy<T>) this).closable.Add(current);
                            }
                            ((DefaultCloseStrategy<T>) this).Evaluate(canClose, enumerator, callback);
                        };
                    }
                    close.CanClose(action);
                }
                else
                {
                    this.closable.Add(current);
                    this.Evaluate(true, enumerator, callback);
                }
            }
        }

        public void Execute(IEnumerable<T> toClose, System.Action<bool, IEnumerable<T>> callback)
        {
            this.finalResult = true;
            this.closable = new List<T>();
            this.Evaluate(true, toClose.GetEnumerator(), callback);
        }
    }
}

