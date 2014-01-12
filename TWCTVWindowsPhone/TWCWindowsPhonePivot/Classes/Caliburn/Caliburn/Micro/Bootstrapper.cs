namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;

    public class Bootstrapper
    {
        private readonly bool isInitialized;
        private readonly bool useApplication;

        public Bootstrapper(bool useApplication = true)
        {
            this.useApplication = useApplication;
            if (!this.isInitialized)
            {
                this.isInitialized = true;
                if (Execute.InDesignMode)
                {
                    try
                    {
                        this.StartDesignTime();
                    }
                    catch
                    {
                        this.isInitialized = false;
                    }
                }
                else
                {
                    this.StartRuntime();
                }
            }
        }

        protected virtual void BuildUp(object instance)
        {
        }

        protected virtual void Configure()
        {
        }

        protected static void DisplayRootViewFor(System.Windows.Application application, Type viewModelType)
        {
            object obj2 = IoC.GetInstance(viewModelType, null);
            UIElement element = ViewLocator.LocateForModel(obj2, null, null);
            ViewModelBinder.Bind(obj2, element, null);
            IActivate activate = obj2 as IActivate;
            if (activate != null)
            {
                activate.Activate();
            }
            Mouse.Initialize(element);
            application.RootVisual = element;
        }

        protected virtual IEnumerable<object> GetAllInstances(Type service)
        {
            return new object[] { Activator.CreateInstance(service) };
        }

        protected virtual object GetInstance(Type service, string key)
        {
            return Activator.CreateInstance(service);
        }

        protected virtual void OnExit(object sender, EventArgs e)
        {
        }

        protected virtual void OnStartup(object sender, StartupEventArgs e)
        {
        }

        protected virtual void OnUnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
        }

        protected virtual void PrepareApplication()
        {
            this.Application.Startup += new StartupEventHandler(this.OnStartup);
            this.Application.UnhandledException += new EventHandler<ApplicationUnhandledExceptionEventArgs>(this.OnUnhandledException);
            this.Application.Exit += new EventHandler(this.OnExit);
        }

        protected virtual IEnumerable<Assembly> SelectAssemblies()
        {
            if (Execute.InDesignMode)
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                Assembly[] assemblyArray = (currentDomain.GetType().GetMethod("GetAssemblies").Invoke(currentDomain, null) as Assembly[]) ?? new Assembly[0];
                return new Assembly[] { (from x in assemblyArray
                    where (x.EntryPoint != null) && Enumerable.Any<Type>(x.GetTypes(), t => t.IsSubclassOf(typeof(System.Windows.Application)))
                    select x).FirstOrDefault<Assembly>() };
            }
            return new Assembly[] { System.Windows.Application.Current.GetType().Assembly };
        }

        protected virtual void StartDesignTime()
        {
            AssemblySource.Instance.AddRange(this.SelectAssemblies());
            this.Configure();
            IoC.GetInstance = new System.Func<Type, string, object>(this.GetInstance);
            IoC.GetAllInstances = new System.Func<Type, IEnumerable<object>>(this.GetAllInstances);
            IoC.BuildUp = new Action<object>(this.BuildUp);
        }

        protected virtual void StartRuntime()
        {
            Execute.InitializeWithDispatcher();
            EventAggregator.DefaultPublicationThreadMarshaller = new Action<System.Action>(Execute.OnUIThread);
            AssemblySource.Instance.AddRange(this.SelectAssemblies());
            if (this.useApplication)
            {
                this.Application = System.Windows.Application.Current;
                this.PrepareApplication();
            }
            this.Configure();
            IoC.GetInstance = new System.Func<Type, string, object>(this.GetInstance);
            IoC.GetAllInstances = new System.Func<Type, IEnumerable<object>>(this.GetAllInstances);
            IoC.BuildUp = new Action<object>(this.BuildUp);
        }

        protected System.Windows.Application Application { get; set; }
    }
}

