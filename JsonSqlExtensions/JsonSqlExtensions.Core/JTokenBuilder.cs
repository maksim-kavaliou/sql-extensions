using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JsonSqlExtensions.Core
{
	public static class JTokenBuilder
	{
		public static JToken Build(string fullKey, string value)
		{
			var newFormatedValue = string.Format("{{\"value\":\"{0}\"}}", value);

			JToken newValueJsonObject = JObject.Parse(newFormatedValue)[Constants.Value];

			var propertiesKeys = JsonPathParser.ParseFullJsonPropertyKey(fullKey);

			return Build(propertiesKeys, newValueJsonObject);
		}

		public static JToken Build(IList<object> keys, JToken resultValue)
		{
			JToken result = resultValue;

			for (int i = keys.Count - 1; i >= 0; i--)
			{
				var key = keys[i];

				// build JObject
				if (key is string)
				{
					result = new JObject(new JProperty((string)key, result));
				}
				else if (key is int?)
				{
					// build JArray
					var index = (int)key;
					var elements = new object[index + 1];

					elements[index] = result;

					result = new JArray(elements);
				}

			}

			return result;
		}
	}
}
