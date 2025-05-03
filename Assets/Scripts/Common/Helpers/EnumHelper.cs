using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

public static class EnumHelper
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

    public static string GetDisplayName<T>(this T value) where T : Enum
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null)
        {
            return value.ToString();
        }

        var displayAttribute = field.GetCustomAttribute<DisplayAttribute>();
        return displayAttribute?.GetName() ?? value.ToString();
    }
}
