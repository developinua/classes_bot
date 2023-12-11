using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Classes.Application.Extensions;

public static class EnumExtensions
{
    public static string DisplayName(this Enum enumValue)
    {
        var displayName = enumValue
            .GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault()
            ?.GetCustomAttribute<DisplayAttribute>(false)
            ?.GetName();

        return string.IsNullOrWhiteSpace(displayName) ? enumValue.ToString() : displayName;
    }
}