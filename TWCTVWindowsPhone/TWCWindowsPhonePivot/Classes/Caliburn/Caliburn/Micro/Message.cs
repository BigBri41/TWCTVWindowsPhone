namespace Caliburn.Micro
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Interactivity;

    public static class Message
    {
        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached("Attach", typeof(string), typeof(Message), new PropertyMetadata(new PropertyChangedCallback(Message.OnAttachChanged)));
        internal static readonly DependencyProperty HandlerProperty = DependencyProperty.RegisterAttached("Handler", typeof(object), typeof(Message), null);
        private static readonly DependencyProperty MessageTriggersProperty = DependencyProperty.RegisterAttached("MessageTriggers", typeof(System.Windows.Interactivity.TriggerBase[]), typeof(Message), null);

        public static string GetAttach(DependencyObject d)
        {
            return (d.GetValue(AttachProperty) as string);
        }

        public static object GetHandler(DependencyObject d)
        {
            return d.GetValue(HandlerProperty);
        }

        private static void OnAttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Action<System.Windows.Interactivity.TriggerBase> action = null;
            System.Windows.Interactivity.TriggerCollection allTriggers;
            if (e.NewValue != e.OldValue)
            {
                System.Windows.Interactivity.TriggerBase[] enumerable = (System.Windows.Interactivity.TriggerBase[]) d.GetValue(MessageTriggersProperty);
                allTriggers = Interaction.GetTriggers(d);
                if (enumerable != null)
                {
                    if (action == null)
                    {
                        action = (Action<System.Windows.Interactivity.TriggerBase>) (x => allTriggers.Remove(x));
                    }
                    enumerable.Apply<System.Windows.Interactivity.TriggerBase>(action);
                }
                System.Windows.Interactivity.TriggerBase[] baseArray2 = Parser.Parse(d, e.NewValue as string).ToArray<System.Windows.Interactivity.TriggerBase>();
                baseArray2.Apply<System.Windows.Interactivity.TriggerBase>(new Action<System.Windows.Interactivity.TriggerBase>(allTriggers.Add));
                if (baseArray2.Length > 0)
                {
                    d.SetValue(MessageTriggersProperty, baseArray2);
                }
                else
                {
                    d.ClearValue(MessageTriggersProperty);
                }
            }
        }

        public static void SetAttach(DependencyObject d, string attachText)
        {
            d.SetValue(AttachProperty, attachText);
        }

        public static void SetHandler(DependencyObject d, object value)
        {
            d.SetValue(HandlerProperty, value);
        }
    }
}

