namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;

    public class WindowManager : IWindowManager
    {
        private bool ApplySettings(object target, IEnumerable<KeyValuePair<string, object>> settings)
        {
            if (settings != null)
            {
                Type type = target.GetType();
                foreach (KeyValuePair<string, object> pair in settings)
                {
                    PropertyInfo property = type.GetProperty(pair.Key);
                    if (property != null)
                    {
                        property.SetValue(target, pair.Value, null);
                    }
                }
                return true;
            }
            return false;
        }

        protected virtual Popup CreatePopup(object rootModel, IDictionary<string, object> settings)
        {
            Popup target = new Popup {
                HorizontalOffset = Mouse.Position.X,
                VerticalOffset = Mouse.Position.Y
            };
            this.ApplySettings(target, settings);
            return target;
        }

        protected virtual ChildWindow EnsureWindow(object model, object view)
        {
            ChildWindow window = view as ChildWindow;
            if (window == null)
            {
                window = new ChildWindow {
                    Content = view
                };
                window.SetValue(View.IsGeneratedProperty, true);
            }
            return window;
        }

        public virtual void ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            ChildWindow window = this.EnsureWindow(rootModel, ViewLocator.LocateForModel(rootModel, null, context));
            ViewModelBinder.Bind(rootModel, window, context);
            if (!(!(rootModel is IHaveDisplayName) || ConventionManager.HasBinding(window, ChildWindow.TitleProperty)))
            {
                Binding binding = new Binding("DisplayName") {
                    Mode = BindingMode.TwoWay
                };
                window.SetBinding(ChildWindow.TitleProperty, binding);
            }
            this.ApplySettings(window, settings);
            new WindowConductor(rootModel, window);
            window.Show();
        }

        public virtual void ShowNotification(object rootModel, int durationInMilliseconds, object context = null, IDictionary<string, object> settings = null)
        {
            EventHandler handler = null;
            NotificationWindow window = new NotificationWindow();
            UIElement element = ViewLocator.LocateForModel(rootModel, window, context);
            ViewModelBinder.Bind(rootModel, element, null);
            window.Content = (FrameworkElement) element;
            this.ApplySettings(window, settings);
            IActivate activate = rootModel as IActivate;
            if (activate != null)
            {
                activate.Activate();
            }
            IDeactivate deactivator = rootModel as IDeactivate;
            if (deactivator != null)
            {
                if (handler == null)
                {
                    handler = (, ) => deactivator.Deactivate(true);
                }
                window.Closed += handler;
            }
            window.Show(durationInMilliseconds);
        }

        public virtual void ShowPopup(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            EventHandler handler = null;
            Popup popup = this.CreatePopup(rootModel, settings);
            UIElement d = ViewLocator.LocateForModel(rootModel, popup, context);
            popup.Child = d;
            popup.SetValue(View.IsGeneratedProperty, true);
            ViewModelBinder.Bind(rootModel, popup, null);
            Caliburn.Micro.Action.SetTargetWithoutContext(d, rootModel);
            IActivate activate = rootModel as IActivate;
            if (activate != null)
            {
                activate.Activate();
            }
            IDeactivate deactivator = rootModel as IDeactivate;
            if (deactivator != null)
            {
                if (handler == null)
                {
                    handler = (, ) => deactivator.Deactivate(true);
                }
                popup.Closed += handler;
            }
            popup.IsOpen = true;
            popup.CaptureMouse();
        }

        private class WindowConductor
        {
            private bool actuallyClosing;
            private bool deactivateFromViewModel;
            private bool deactivatingFromView;
            private readonly object model;
            private readonly ChildWindow view;

            public WindowConductor(object model, ChildWindow view)
            {
                this.model = model;
                this.view = view;
                IActivate activate = model as IActivate;
                if (activate != null)
                {
                    activate.Activate();
                }
                IDeactivate deactivate = model as IDeactivate;
                if (deactivate != null)
                {
                    view.Closed += new EventHandler(this.Closed);
                    deactivate.Deactivated += new EventHandler<DeactivationEventArgs>(this.Deactivated);
                }
                IGuardClose close = model as IGuardClose;
                if (close != null)
                {
                    view.Closing += new EventHandler<CancelEventArgs>(this.Closing);
                }
            }

            private void Closed(object sender, EventArgs e)
            {
                this.view.Closed -= new EventHandler(this.Closed);
                this.view.Closing -= new EventHandler<CancelEventArgs>(this.Closing);
                if (!this.deactivateFromViewModel)
                {
                    IDeactivate model = (IDeactivate) this.model;
                    this.deactivatingFromView = true;
                    model.Deactivate(true);
                    this.deactivatingFromView = false;
                }
            }

            private void Closing(object sender, CancelEventArgs e)
            {
                bool runningAsync;
                bool shouldEnd;
                if (!e.Cancel)
                {
                    IGuardClose model = (IGuardClose) this.model;
                    if (this.actuallyClosing)
                    {
                        this.actuallyClosing = false;
                    }
                    else
                    {
                        runningAsync = false;
                        shouldEnd = false;
                        model.CanClose(canClose => delegate {
                            if (runningAsync && canClose)
                            {
                                this.actuallyClosing = true;
                                this.view.Close();
                            }
                            else
                            {
                                e.Cancel = !canClose;
                            }
                            shouldEnd = true;
                        }.OnUIThread());
                        if (!shouldEnd)
                        {
                            runningAsync = e.Cancel = true;
                        }
                    }
                }
            }

            private void Deactivated(object sender, DeactivationEventArgs e)
            {
                if (e.WasClosed)
                {
                    ((IDeactivate) this.model).Deactivated -= new EventHandler<DeactivationEventArgs>(this.Deactivated);
                    if (!this.deactivatingFromView)
                    {
                        this.deactivateFromViewModel = true;
                        this.actuallyClosing = true;
                        this.view.Close();
                        this.actuallyClosing = false;
                        this.deactivateFromViewModel = false;
                    }
                }
            }
        }
    }
}

