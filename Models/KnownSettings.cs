using System;

namespace WebEdit.Models;

public record KnownSetting(string Label, IReadOnlyList<string> AllowedValues, string Key, string DefaultValue);
