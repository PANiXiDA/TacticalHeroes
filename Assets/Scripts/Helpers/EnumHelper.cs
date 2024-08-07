using System;
using System.ComponentModel;
using System.Reflection;

namespace Assets.Scripts.Helpers
{
    public class EnumHelper
    {
        public static string GetDescription<TEnum>(TEnum ability) where TEnum : Enum
        {
            Type enumType = typeof(TEnum);
            FieldInfo fieldInfo = enumType.GetField(ability.ToString());
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));
            if (attribute != null)
            {
                return attribute.Description;
            }
            return nameof(ability);
        }
    }
}
