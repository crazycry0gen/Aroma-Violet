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

        public static t GetSetting<t>(this AromaContext context, string key)
        {
            var val = default(t);

            var settingEntry = (from item in context.SystemSettings
                                where item.SettingKey == key
                                select item).FirstOrDefault();
            if (settingEntry == null)
            {
                settingEntry = new SystemSetting() { SettingKey = key, SettingValue = val.ToString() };
                context.SystemSettings.Add(settingEntry);
                context.SaveChanges();
                return val;
            }
            return context.GetSetting<t>(settingEntry.SettingId);
        }

        public static t GetSetting<t>(this AromaContext context, int keyId)
        {
            var val = default(t);
            var type = typeof(t);
            var settingEntry = (from item in context.SystemSettings
                                where item.SettingId == keyId
                                select item).First();
            try
            {
                val = (t)(object)settingEntry.SettingValue;
            }
            catch  { }
            return val;
        }

        public static void Log<t>(this AromaContext context, enumLGActivity activity, t Id, string note)
        {
            if (context.GetSetting<bool>(1))
            {
                var newEntry = new LGActivityLog() { ActivityId = (int)activity, Note = note, iDate = DateTime.Now };
                if (typeof(t) == typeof(int)) newEntry.IntId = (int)(object)Id;
                if (typeof(t) == typeof(Guid)) newEntry.GuidId = (Guid)(object)Id;
                context.ActivityLogs.Add(newEntry);
                context.SaveChanges();
            }
        }

        public enum enumLGActivity:int
        {
            CreateClient=1,
            UpdateClient=2
        }
    }
}