namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Interactivity;
    using System.Windows.Markup;
    using System.Windows.Media;

    [DefaultTrigger(typeof(ButtonBase), typeof(System.Windows.Interactivity.EventTrigger), "Click"), DefaultTrigger(typeof(FrameworkElement), typeof(System.Windows.Interactivity.EventTrigger), "MouseLeftButtonDown"), TypeConstraint(typeof(FrameworkElement)), ContentProperty("Parameters")]
    public class ActionMessage : TriggerAction<FrameworkElement>, IHaveParameters
    {
        public static System.Func<ActionExecutionContext, bool> ApplyAvailabilityEffect;
        private ActionExecutionContext context;
        public static bool EnforceGuardsDuringInvocation = false;
        public static System.Func<ActionMessage, object, MethodInfo> GetTargetMethod;
        internal static readonly DependencyProperty HandlerProperty = DependencyProperty.RegisterAttached("Handler", typeof(object), typeof(ActionMessage), new PropertyMetadata(new PropertyChangedCallback(ActionMessage.HandlerPropertyChanged)));
        public static Action<ActionExecutionContext> InvokeAction;
        private static readonly ILog Log = LogManager.GetLog(typeof(ActionMessage));
        public static readonly DependencyProperty MethodNameProperty = DependencyProperty.Register("MethodName", typeof(string), typeof(ActionMessage), null);
        public static readonly DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(AttachedCollection<Parameter>), typeof(ActionMessage), null);
        public static Action<ActionExecutionContext> PrepareContext;
        public static Action<ActionExecutionContext> SetMethodBinding;
        public static bool ThrowsExceptions = true;

        public event EventHandler Detaching;

        static ActionMessage()
        {
            InvokeAction = delegate (ActionExecutionContext context) {
                object[] parameters = MessageBinder.DetermineParameters(context, context.Method.GetParameters());
                object obj2 = context.Method.Invoke(context.Target, parameters);
                IResult result = obj2 as IResult;
                if (result != null)
                {
                    obj2 = new IResult[] { result };
                }
                IEnumerable<IResult> enumerable = obj2 as IEnumerable<IResult>;
                if (enumerable != null)
                {
                    Coroutine.BeginExecute(enumerable.GetEnumerator(), context, null);
                }
                else if (obj2 is IEnumerator<IResult>)
                {
                    Coroutine.BeginExecute((IEnumerator<IResult>) obj2, context, null);
                }
            };
            ApplyAvailabilityEffect = delegate (ActionExecutionContext context) {
                if (!(context.Source is Control))
                {
                    return true;
                }
                Control element = (Control) context.Source;
                if (!ConventionManager.HasBinding(element, Control.IsEnabledProperty) && (context.CanExecute != null))
                {
                    element.IsEnabled = context.CanExecute();
                }
                return element.IsEnabled;
            };
            GetTargetMethod = (message, target) => (from method in target.GetType().GetMethods()
                where method.Name == message.MethodName
                let methodParameters = method.GetParameters()
                where message.Parameters.Count == methodParameters.Length
                select method).FirstOrDefault<MethodInfo>();
            SetMethodBinding = delegate (ActionExecutionContext context) {
                object dataContext;
                MethodInfo info;
                for (DependencyObject obj2 = context.Source; obj2 != null; obj2 = VisualTreeHelper.GetParent(obj2))
                {
                    if (Caliburn.Micro.Action.HasTargetSet(obj2))
                    {
                        dataContext = Message.GetHandler(obj2);
                        if (dataContext != null)
                        {
                            info = GetTargetMethod(context.Message, dataContext);
                            if (info != null)
                            {
                                context.Method = info;
                                context.Target = dataContext;
                                context.View = obj2;
                                return;
                            }
                        }
                        else
                        {
                            context.View = obj2;
                            return;
                        }
                    }
                }
                if (context.Source.DataContext != null)
                {
                    dataContext = context.Source.DataContext;
                    info = GetTargetMethod(context.Message, dataContext);
                    if (info != null)
                    {
                        context.Target = dataContext;
                        context.Method = info;
                        context.View = context.Source;
                    }
                }
            };
            PrepareContext = delegate (ActionExecutionContext context) {
                SetMethodBinding(context);
                if ((context.Target != null) && (context.Method != null))
                {
                    string guardName = "Can" + context.Method.Name;
                    Type type = context.Target.GetType();
                    MethodInfo guard = TryFindGuardMethod(context);
                    if (guard == null)
                    {
                        INotifyPropertyChanged inpc = context.Target as INotifyPropertyChanged;
                        if (inpc == null)
                        {
                            return;
                        }
                        guard = type.GetMethod("get_" + guardName);
                        if (guard == null)
                        {
                            return;
                        }
                        PropertyChangedEventHandler handler = null;
                        handler = delegate (object s, PropertyChangedEventArgs e) {
                            if (string.IsNullOrEmpty(e.PropertyName) || (e.PropertyName == guardName))
                            {
                                if (context.Message == null)
                                {
                                    inpc.PropertyChanged -= handler;
                                }
                                else
                                {
                                    context.Message.UpdateAvailability();
                                }
                            }
                        };
                        inpc.PropertyChanged += handler;
                        context.Disposing += delegate {
                            inpc.PropertyChanged -= handler;
                        };
                        context.Message.Detaching += delegate {
                            inpc.PropertyChanged -= handler;
                        };
                    }
                    context.CanExecute = () => (bool) guard.Invoke(context.Target, MessageBinder.DetermineParameters(context, guard.GetParameters()));
                }
            };
        }

        public ActionMessage()
        {
            base.SetValue(ParametersProperty, new AttachedCollection<Parameter>());
        }

        private void ElementLoaded(object sender, RoutedEventArgs e)
        {
            DependencyObject view;
            this.UpdateContext();
            if (this.context.View == null)
            {
                for (view = base.AssociatedObject; view != null; view = VisualTreeHelper.GetParent(view))
                {
                    if (Caliburn.Micro.Action.HasTargetSet(view))
                    {
                        break;
                    }
                }
            }
            else
            {
                view = this.context.View;
            }
            Binding binding = (Binding) XamlReader.Load("<Binding xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:cal='clr-namespace:Caliburn.Micro;assembly=Caliburn.Micro' Path='(cal:Message.Handler)' />");
            binding.Source = view;
            BindingOperations.SetBinding(this, HandlerProperty, binding);
        }

        private static void HandlerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ActionMessage) d).UpdateContext();
        }

        protected override void Invoke(object eventArgs)
        {
            Exception exception;
            Log.Info("Invoking {0}.", new object[] { this });
            if (this.context == null)
            {
                this.UpdateContext();
            }
            if ((this.context.Target == null) || (this.context.View == null))
            {
                PrepareContext(this.context);
                if (this.context.Target == null)
                {
                    exception = new Exception(string.Format("No target found for method {0}.", this.context.Message.MethodName));
                    Log.Error(exception);
                    if (ThrowsExceptions)
                    {
                        throw exception;
                    }
                    return;
                }
                if (!this.UpdateAvailabilityCore())
                {
                    return;
                }
            }
            if (this.context.Method == null)
            {
                exception = new Exception(string.Format("Method {0} not found on target of type {1}.", this.context.Message.MethodName, this.context.Target.GetType()));
                Log.Error(exception);
                if (ThrowsExceptions)
                {
                    throw exception;
                }
            }
            else
            {
                this.context.EventArgs = eventArgs;
                if ((!EnforceGuardsDuringInvocation || (this.context.CanExecute == null)) ? true : this.context.CanExecute())
                {
                    InvokeAction(this.context);
                    this.context.EventArgs = null;
                }
            }
        }

        protected override void OnAttached()
        {
            Action<Parameter> action = null;
            System.Func<System.Windows.Interactivity.TriggerBase, bool> func = null;
            if (!Execute.InDesignMode)
            {
                this.Parameters.Attach(base.AssociatedObject);
                if (action == null)
                {
                    action = x => x.MakeAwareOf(this);
                }
                this.Parameters.Apply<Parameter>(action);
                if (View.ExecuteOnLoad(base.AssociatedObject, new RoutedEventHandler(this.ElementLoaded)))
                {
                    if (func == null)
                    {
                        func = t => t.Actions.Contains(this);
                    }
                    System.Windows.Interactivity.EventTrigger trigger = Enumerable.FirstOrDefault<System.Windows.Interactivity.TriggerBase>(Interaction.GetTriggers(base.AssociatedObject), func) as System.Windows.Interactivity.EventTrigger;
                    if ((trigger != null) && (trigger.EventName == "Loaded"))
                    {
                        this.Invoke(new RoutedEventArgs());
                    }
                }
            }
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            if (!Execute.InDesignMode)
            {
                this.Detaching(this, EventArgs.Empty);
                base.AssociatedObject.Loaded -= new RoutedEventHandler(this.ElementLoaded);
                this.Parameters.Detach();
            }
            base.OnDetaching();
        }

        public override string ToString()
        {
            return ("Action: " + this.MethodName);
        }

        private static MethodInfo TryFindGuardMethod(ActionExecutionContext context)
        {
            string name = "Can" + context.Method.Name;
            MethodInfo method = context.Target.GetType().GetMethod(name);
            if (method == null)
            {
                return null;
            }
            if (method.ContainsGenericParameters)
            {
                return null;
            }
            if (!typeof(bool).Equals(method.ReturnType))
            {
                return null;
            }
            ParameterInfo[] parameters = method.GetParameters();
            ParameterInfo[] infoArray2 = context.Method.GetParameters();
            if (parameters.Length != 0)
            {
                if (parameters.Length != infoArray2.Length)
                {
                    return null;
                }
                if (Enumerable.Any<bool>(Enumerable.Zip<ParameterInfo, ParameterInfo, bool>(parameters, context.Method.GetParameters(), (x, y) => x.ParameterType.Equals(y.ParameterType)), x => !x))
                {
                    return null;
                }
            }
            return method;
        }

        public void UpdateAvailability()
        {
            if (this.context != null)
            {
                if ((this.context.Target == null) || (this.context.View == null))
                {
                    PrepareContext(this.context);
                }
                this.UpdateAvailabilityCore();
            }
        }

        private bool UpdateAvailabilityCore()
        {
            Log.Info("{0} availability update.", new object[] { this });
            return ApplyAvailabilityEffect(this.context);
        }

        private void UpdateContext()
        {
            if (this.context != null)
            {
                this.context.Dispose();
            }
            ActionExecutionContext context = new ActionExecutionContext {
                Message = this,
                Source = base.AssociatedObject
            };
            this.context = context;
            PrepareContext(this.context);
            this.UpdateAvailabilityCore();
        }

        [Category("Common Properties")]
        public string MethodName
        {
            get
            {
                return (string) base.GetValue(MethodNameProperty);
            }
            set
            {
                base.SetValue(MethodNameProperty, value);
            }
        }

        [Category("Common Properties")]
        public AttachedCollection<Parameter> Parameters
        {
            get
            {
                return (AttachedCollection<Parameter>) base.GetValue(ParametersProperty);
            }
        }
    }
}

