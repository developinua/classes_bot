using System;
using Microsoft.Extensions.Localization;

namespace Classes.Application.Extensions;

public static class StringLocalizerExtensions
{
    public static string WithNewLines(this LocalizedString localizedString) =>
        localizedString.Value.Replace("{newline}", Environment.NewLine);
}