namespace Caliburn.Micro
{
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    public static class Execute
    {
        private static Action<System.Action> executor;
        private static bool? inDesignMode;

        static Execute()
        {
            executor = action => action();
        }

        public static void InitializeWithDispatcher()
        {
            Dispatcher dispatcher = Deployment.Current.Dispatcher;
            SetUIThreadMarshaller(delegate (System.Action action) {
                if (dispatcher.CheckAccess())
                {
                    action();
                }
                else
                {
                    ManualResetEvent waitHandle = new ManualResetEvent(false);
                    Exception exception = null;
                    dispatcher.BeginInvoke(delegate {
                        try
                        {
                            action();
                        }
                        catch (Exception exception1)
                        {
                            exception = exception1;
                        }
                        waitHandle.Set();
                    });
                    waitHandle.WaitOne();
                    if (exception != null)
                    {
                        throw new TargetInvocationException("An error occurred while dispatching a call to the UI Thread", exception);
                    }
                }
            });
        }

        public static void OnUIThread(this System.Action action)
        {
            executor(action);
        }

        public static void ResetWithoutDispatcher()
        {
            SetUIThreadMarshaller(action => action());
        }

        public static void SetUIThreadMarshaller(Action<System.Action> marshaller)
        {
            executor = marshaller;
        }

        public static bool InDesignMode
        {
            get
            {
                if (!inDesignMode.HasValue)
                {
                    inDesignMode = new bool?(DesignerProperties.IsInDesignTool);
                }
                return inDesignMode.GetValueOrDefault(false);
            }
        }
    }
}

