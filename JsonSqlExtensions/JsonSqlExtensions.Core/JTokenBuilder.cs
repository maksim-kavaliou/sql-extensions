using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSqlExtensions.Core
{
	public static class JTokenBuilder
	{
		public static JToken Build(string value)
		{
			JToken newValueJsonObject;

			try
			{
				// JConvert allow to deserialize objects and arrays
				object jsonObject = JsonConvert.DeserializeObject(value);

				var jObject = JObject.FromObject(new {value = jsonObject});

				newValueJsonObject = jObject[Constants.Value];
			}
			catch (Exception e)
			{
				// value is usual string
				newValueJsonObject = new JValue(value);
			}

			return newValueJsonObject;
		}
	}
}
