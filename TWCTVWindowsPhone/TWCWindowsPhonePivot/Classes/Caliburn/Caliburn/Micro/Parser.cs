namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Interactivity;

    public static class Parser
    {
        public static System.Func<DependencyObject, string, Parameter> CreateParameter;
        public static System.Func<DependencyObject, string, System.Windows.Interactivity.TriggerBase> CreateTrigger;
        public static System.Func<DependencyObject, string, System.Windows.Interactivity.TriggerAction> InterpretMessageText;
        private static readonly Regex LongFormatRegularExpression = new Regex(@"^[\s]*\[[^\]]*\][\s]*=[\s]*\[[^\]]*\][\s]*$");

        static Parser()
        {
            CreateTrigger = delegate (DependencyObject target, string triggerText) {
                if (triggerText == null)
                {
                    return ConventionManager.GetElementConvention(target.GetType()).CreateTrigger();
                }
                string str = triggerText.Replace("[", string.Empty).Replace("]", string.Empty).Replace("Event", string.Empty).Trim();
                return new System.Windows.Interactivity.EventTrigger { EventName = str };
            };
            InterpretMessageText = (target, text) => new ActionMessage { MethodName = Regex.Replace(text, "^Action", string.Empty).Trim() };
            CreateParameter = delegate (DependencyObject target, string parameterText) {
                Parameter actualParameter = new Parameter();
                if (parameterText.StartsWith("'") && parameterText.EndsWith("'"))
                {
                    actualParameter.Value = parameterText.Substring(1, parameterText.Length - 2);
                }
                else if (MessageBinder.SpecialValues.ContainsKey(parameterText.ToLower()) || char.IsNumber(parameterText[0]))
                {
                    actualParameter.Value = parameterText;
                }
                else if (target is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement) target;
                    string[] nameAndBindingMode = (from x in parameterText.Split(new char[] { ':' }) select x.Trim()).ToArray<string>();
                    int index = nameAndBindingMode[0].IndexOf('.');
                    View.ExecuteOnLoad(fe, (, ) => BindParameter(fe, actualParameter, (index == -1) ? nameAndBindingMode[0] : nameAndBindingMode[0].Substring(0, index), (index == -1) ? null : nameAndBindingMode[0].Substring(index + 1), (nameAndBindingMode.Length == 2) ? ((BindingMode) Enum.Parse(typeof(BindingMode), nameAndBindingMode[1], true)) : BindingMode.OneWay));
                }
                return actualParameter;
            };
        }

        public static void BindParameter(FrameworkElement target, Parameter parameter, string elementName, string path, BindingMode bindingMode)
        {
            FrameworkElement element = (elementName == "$this") ? target : BindingScope.GetNamedElements(target).FindName(elementName);
            if (element != null)
            {
                if (string.IsNullOrEmpty(path))
                {
                    path = ConventionManager.GetElementConvention(element.GetType()).ParameterProperty;
                }
                Binding binding = new Binding(path) {
                    Source = element,
                    Mode = bindingMode
                };
                binding.UpdateSourceTrigger = (UpdateSourceTrigger) 1;
                BindingOperations.SetBinding(parameter, Parameter.ValueProperty, binding);
            }
        }

        public static System.Windows.Interactivity.TriggerAction CreateMessage(DependencyObject target, string messageText)
        {
            int index = messageText.IndexOf('(');
            if (index < 0)
            {
                index = messageText.Length;
            }
            int length = messageText.LastIndexOf(')');
            if (length < 0)
            {
                length = messageText.Length;
            }
            string str = messageText.Substring(0, index).Trim();
            System.Windows.Interactivity.TriggerAction action = InterpretMessageText(target, str);
            IHaveParameters parameters = action as IHaveParameters;
            if (parameters != null)
            {
                if ((length - index) <= 1)
                {
                    return action;
                }
                string[] strArray = SplitParameters(messageText.Substring(index + 1, (length - index) - 1));
                foreach (string str3 in strArray)
                {
                    parameters.Parameters.Add(CreateParameter(target, str3.Trim()));
                }
            }
            return action;
        }

        public static IEnumerable<System.Windows.Interactivity.TriggerBase> Parse(DependencyObject target, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new System.Windows.Interactivity.TriggerBase[0];
            }
            List<System.Windows.Interactivity.TriggerBase> list = new List<System.Windows.Interactivity.TriggerBase>();
            string[] strArray = Split(text, ';');
            foreach (string str in strArray)
            {
                string[] source = LongFormatRegularExpression.IsMatch(str) ? Split(str, '=') : new string[2];
                string messageText = source.Last<string>().Replace("[", string.Empty).Replace("]", string.Empty).Trim();
                System.Windows.Interactivity.TriggerBase item = CreateTrigger(target, (source.Length == 1) ? null : source[0]);
                System.Windows.Interactivity.TriggerAction action = CreateMessage(target, messageText);
                item.Actions.Add(action);
                list.Add(item);
            }
            return list;
        }

        private static string[] Split(string message, char separator)
        {
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            int num = 0;
            foreach (char ch in message)
            {
                if (ch == '[')
                {
                    num++;
                }
                else if (ch == ']')
                {
                    num--;
                }
                else if ((ch == separator) && (num == 0))
                {
                    if (!string.IsNullOrEmpty(builder.ToString()))
                    {
                        list.Add(builder.ToString());
                    }
                    builder.Length = 0;
                    continue;
                }
                builder.Append(ch);
            }
            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                list.Add(builder.ToString());
            }
            return list.ToArray();
        }

        private static string[] SplitParameters(string parameters)
        {
            List<string> list = new List<string>();
            StringBuilder builder = new StringBuilder();
            bool flag = false;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            for (int i = 0; i < parameters.Length; i++)
            {
                char ch = parameters[i];
                if ((ch == '"') && ((i == 0) || (parameters[i - 1] != '\\')))
                {
                    flag = !flag;
                }
                if (!flag)
                {
                    switch (ch)
                    {
                        case '(':
                            num3++;
                            goto Label_0111;

                        case ')':
                            num3--;
                            goto Label_0111;

                        case '[':
                            num2++;
                            goto Label_0111;

                        case ']':
                            num2--;
                            goto Label_0111;

                        case '{':
                            num++;
                            goto Label_0111;

                        case '}':
                            num--;
                            goto Label_0111;
                    }
                    if ((((ch == ',') && (num3 == 0)) && (num2 == 0)) && (num == 0))
                    {
                        list.Add(builder.ToString());
                        builder.Length = 0;
                        continue;
                    }
                }
            Label_0111:
                builder.Append(ch);
            }
            list.Add(builder.ToString());
            return list.ToArray();
        }
    }
}

