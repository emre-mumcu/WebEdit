using System;
using Newtonsoft.Json;

namespace WebEdit.AppLib;

public static class SessionExtension
{
	private static JsonSerializerSettings jsonSerializerSettings
	{
		get
		{
			return new JsonSerializerSettings()
			{
				Formatting = Formatting.None,
				PreserveReferencesHandling = PreserveReferencesHandling.None,
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
				NullValueHandling = NullValueHandling.Include,
				Culture = new System.Globalization.CultureInfo("tr-TR")
			};
		}
	}

	public static void SetKey<T>(this ISession session, string key, T? value)
	{
		session.SetString(key, JsonConvert.SerializeObject(value, Formatting.None, jsonSerializerSettings));
	}

	public static T? GetKey<T>(this ISession session, string key)
	{
		string? value = session.GetString(key);
		return value == null ? default : JsonConvert.DeserializeObject<T>(value);
	}

	public static T? TakeKey<T>(this ISession session, string key)
	{
		string? value = session.GetString(key);
		session.SetKey<T>(key, default);
		return value == null ? default : JsonConvert.DeserializeObject<T>(value);
	}

	public static void RemoveKey(this ISession session, string key)
	{
		session.Remove(key);
	}

	public static string GetSessionId(this ISession session) => session.Id;
}
