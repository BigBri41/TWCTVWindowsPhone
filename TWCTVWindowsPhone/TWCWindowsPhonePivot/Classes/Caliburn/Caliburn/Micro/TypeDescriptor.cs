namespace Caliburn.Micro
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    public static class TypeDescriptor
    {
        private static readonly Dictionary<Type, TypeConverter> Cache = new Dictionary<Type, TypeConverter>();

        public static TypeConverter GetConverter(Type type)
        {
            TypeConverter converter;
            if (!Cache.TryGetValue(type, out converter))
            {
                IEnumerable<TypeConverterAttribute> attributes = type.GetAttributes<TypeConverterAttribute>(true);
                if (!attributes.Any<TypeConverterAttribute>())
                {
                    return new TypeConverter();
                }
                converter = Activator.CreateInstance(Type.GetType(attributes.First<TypeConverterAttribute>().ConverterTypeName)) as TypeConverter;
                Cache[type] = converter;
            }
            return converter;
        }
    }
}

