using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Aroma_Violet.Models
{
    public static class Generic
    {
        public static t CopyObject<t>(t fromObject, t toObject, params string[] dontCopy)
        {
            var type = typeof(t);
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                if (!dontCopy.Contains(property.Name))
                {
                    var value = property.GetValue(fromObject);
                    var defaultValue = default(t);
                    if (value != null && !value.Equals(defaultValue))
                    {
                        property.SetValue(toObject, value);
                    }
                }
            }
            return toObject;
        }
    }
}