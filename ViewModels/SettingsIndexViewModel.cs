using System;
using WebEdit.Models;

namespace WebEdit.ViewModels;

public class SettingsIndexViewModel
{
	public IReadOnlyList<KnownSetting> KnownSettings { get; init; } = [];
	public Dictionary<string, string> SavedValues { get; init; } = [];

	public string GetValue(string key) =>
		SavedValues.TryGetValue(key, out var v) ? v : "";
}
