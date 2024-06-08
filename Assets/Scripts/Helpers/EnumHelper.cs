using Assets.Scripts.Enumeration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
            return "Описание не найдено";
        }
    }
}
