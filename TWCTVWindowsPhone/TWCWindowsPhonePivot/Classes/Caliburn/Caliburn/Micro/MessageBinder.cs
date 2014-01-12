namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    public static class MessageBinder
    {
        public static readonly Dictionary<Type, System.Func<object, object, object>> CustomConverters;
        public static System.Func<string, Type, ActionExecutionContext, object> EvaluateParameter;
        public static readonly Dictionary<string, System.Func<ActionExecutionContext, object>> SpecialValues;

        static MessageBinder()
        {
            Dictionary<string, System.Func<ActionExecutionContext, object>> dictionary = new Dictionary<string, System.Func<ActionExecutionContext, object>>();
            dictionary.Add("$eventargs", c => c.EventArgs);
            dictionary.Add("$datacontext", c => c.Source.DataContext);
            dictionary.Add("$source", c => c.Source);
            dictionary.Add("$executioncontext", c => c);
            dictionary.Add("$view", c => c.View);
            SpecialValues = dictionary;
            Dictionary<Type, System.Func<object, object, object>> dictionary2 = new Dictionary<Type, System.Func<object, object, object>>();
            dictionary2.Add(typeof(DateTime), delegate (object value, object context) {
                DateTime time;
                DateTime.TryParse(value.ToString(), out time);
                return time;
            });
            CustomConverters = dictionary2;
            EvaluateParameter = delegate (string text, Type parameterType, ActionExecutionContext context) {
                System.Func<ActionExecutionContext, object> func;
                string key = text.ToLower(CultureInfo.InvariantCulture);
                return SpecialValues.TryGetValue(key, out func) ? func(context) : text;
            };
        }

        public static object CoerceValue(Type destinationType, object providedValue, object context)
        {
            if (providedValue == null)
            {
                return GetDefaultValue(destinationType);
            }
            Type c = providedValue.GetType();
            if (destinationType.IsAssignableFrom(c))
            {
                return providedValue;
            }
            if (CustomConverters.ContainsKey(destinationType))
            {
                return CustomConverters[destinationType](providedValue, context);
            }
            try
            {
                string str;
                TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
                if (converter.CanConvertFrom(c))
                {
                    return converter.ConvertFrom(providedValue);
                }
                converter = TypeDescriptor.GetConverter(c);
                if (converter.CanConvertTo(destinationType))
                {
                    return converter.ConvertTo(providedValue, destinationType);
                }
                if (destinationType.IsEnum)
                {
                    str = providedValue as string;
                    if (str != null)
                    {
                        return Enum.Parse(destinationType, str, true);
                    }
                    return Enum.ToObject(destinationType, providedValue);
                }
                if (typeof(Guid).IsAssignableFrom(destinationType))
                {
                    str = providedValue as string;
                    if (str != null)
                    {
                        return new Guid(str);
                    }
                }
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }
            try
            {
                return Convert.ChangeType(providedValue, destinationType, CultureInfo.CurrentUICulture);
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }
        }

        public static object[] DetermineParameters(ActionExecutionContext context, ParameterInfo[] requiredParameters)
        {
            object[] objArray = (from x in context.Message.Parameters select x.Value).ToArray<object>();
            object[] objArray2 = new object[requiredParameters.Length];
            for (int i = 0; i < requiredParameters.Length; i++)
            {
                Type parameterType = requiredParameters[i].ParameterType;
                object providedValue = objArray[i];
                string str = providedValue as string;
                if (str != null)
                {
                    objArray2[i] = CoerceValue(parameterType, EvaluateParameter(str, parameterType, context), context);
                }
                else
                {
                    objArray2[i] = CoerceValue(parameterType, providedValue, context);
                }
            }
            return objArray2;
        }

        public static object GetDefaultValue(Type type)
        {
            return ((type.IsClass || type.IsInterface) ? null : Activator.CreateInstance(type));
        }
    }
}

