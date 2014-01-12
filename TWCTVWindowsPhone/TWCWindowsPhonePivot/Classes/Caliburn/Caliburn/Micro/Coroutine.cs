namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading;

    public static class Coroutine
    {
        public static System.Func<IEnumerator<IResult>, IResult> CreateParentEnumerator = inner => new SequentialResult(inner);
        private static readonly ILog Log = LogManager.GetLog(typeof(Coroutine));

        public static  event EventHandler<ResultCompletionEventArgs> Completed;

        static Coroutine()
        {
            Completed = delegate (object s, ResultCompletionEventArgs e) {
                IResult result = (IResult) s;
                result.Completed -= Completed;
                if (e.Error != null)
                {
                    Log.Error(e.Error);
                }
                else if (e.WasCancelled)
                {
                    Log.Info("Coroutine execution cancelled.", new object[0]);
                }
                else
                {
                    Log.Info("Coroutine execution completed.", new object[0]);
                }
            };
        }

        public static void BeginExecute(IEnumerator<IResult> coroutine, ActionExecutionContext context = null, EventHandler<ResultCompletionEventArgs> callback = null)
        {
            Log.Info("Executing coroutine.", new object[0]);
            IResult result = CreateParentEnumerator(coroutine);
            IoC.BuildUp(result);
            if (callback != null)
            {
                result.Completed += callback;
            }
            result.Completed += Completed;
            result.Execute(context ?? new ActionExecutionContext());
        }
    }
}

