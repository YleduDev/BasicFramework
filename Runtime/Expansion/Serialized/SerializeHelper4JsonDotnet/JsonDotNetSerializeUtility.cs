using UnityEngine;

namespace Framework
{
	// 为了防止进行 clrbidning
	using Newtonsoft.Json;

	public static class JsonDotnetSerializeUtility
	{
		public static string SerializeJson<T>(this T obj) where T : class
		{
			return JsonConvert.SerializeObject(obj, Formatting.None);
		}

		public static T DeserializeJson<T>(this string json) where T : class
		{
			return JsonConvert.DeserializeObject<T>(json);
		}

		public static string ToJson<T>(this T obj)
		{
			return JsonConvert.SerializeObject(obj, Formatting.None);
		}

	}
}