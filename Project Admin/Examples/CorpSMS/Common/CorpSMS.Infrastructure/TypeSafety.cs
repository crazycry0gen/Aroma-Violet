using System;

namespace StratCorp.CorpSMS
{
    public static class TypeSafety
    {
        public static T GetValue<T>(object value)
        {
            if (value is T)
            {
                return (T)value;
            }
            else
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch (InvalidCastException)
                {
                    return default(T);
                }
            }
        }

        public static T GetValue<T>(object value, T defaultValue)
        {
            if (value is T)
            {
                return (T)value;
            }
            else
            {
                try
                {
                    T converted = (T)Convert.ChangeType(value, typeof(T));
                    if (converted == null)
                    {
                        return defaultValue;
                    }
                    return converted;
                }
                catch (InvalidCastException)
                {
                    return defaultValue;
                }
                catch (FormatException)
                {
                    return defaultValue;
                }
            }
        }
    }
}
