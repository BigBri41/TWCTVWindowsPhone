namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class SequentialResult : IResult
    {
        private ActionExecutionContext context;
        private readonly IEnumerator<IResult> enumerator;

        public event EventHandler<ResultCompletionEventArgs> Completed;

        public SequentialResult(IEnumerator<IResult> enumerator)
        {
            this.enumerator = enumerator;
        }

        private void ChildCompleted(object sender, ResultCompletionEventArgs args)
        {
            IResult result = sender as IResult;
            if (result != null)
            {
                result.Completed -= new EventHandler<ResultCompletionEventArgs>(this.ChildCompleted);
            }
            if ((args.Error != null) || args.WasCancelled)
            {
                this.OnComplete(args.Error, args.WasCancelled);
            }
            else
            {
                Exception exception;
                bool flag = false;
                try
                {
                    flag = this.enumerator.MoveNext();
                }
                catch (Exception exception1)
                {
                    exception = exception1;
                    this.OnComplete(exception, false);
                    return;
                }
                if (flag)
                {
                    try
                    {
                        IResult current = this.enumerator.Current;
                        IoC.BuildUp(current);
                        current.Completed += new EventHandler<ResultCompletionEventArgs>(this.ChildCompleted);
                        current.Execute(this.context);
                    }
                    catch (Exception exception2)
                    {
                        exception = exception2;
                        this.OnComplete(exception, false);
                    }
                }
                else
                {
                    this.OnComplete(null, false);
                }
            }
        }

        public void Execute(ActionExecutionContext context)
        {
            this.context = context;
            this.ChildCompleted(null, new ResultCompletionEventArgs());
        }

        private void OnComplete(Exception error, bool wasCancelled)
        {
            this.enumerator.Dispose();
            ResultCompletionEventArgs e = new ResultCompletionEventArgs {
                Error = error,
                WasCancelled = wasCancelled
            };
            this.Completed(this, e);
        }
    }
}

