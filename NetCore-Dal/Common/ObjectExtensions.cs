using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace YDal.Common
{
    public static class ObjectExtensions
    {
        public static object As(this object value, Type targetType, bool throwIfError = false)
        {
            Type underlyingType = null;

            try
            {
                if (value == null) return null;

                underlyingType = Nullable.GetUnderlyingType(targetType);
                Type usedTargetType = underlyingType ?? targetType;

                Type sourceType = value.GetType();
                if (sourceType == underlyingType) return value;

                TypeConverter converter = TypeDescriptor.GetConverter(usedTargetType);
                if (converter.CanConvertFrom(sourceType))
                {
                    return converter.ConvertFrom(value);
                }

                converter = TypeDescriptor.GetConverter(sourceType);
                if (converter.CanConvertTo(usedTargetType))
                {
                    return converter.ConvertTo(value, usedTargetType);
                }

                return value;
            }
            catch
            {
                if (throwIfError) throw;
                // We try best to resolve but it will hide the original error.
                if (underlyingType == null)
                {
                    return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
                }

                return null;
            }
        }
    }
}
