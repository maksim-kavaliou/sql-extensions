using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSqlExtensions.Core
{
	public static class JTokenExtensions
	{
		public static string ToJsonString(this JToken jToken)
		{
			if (jToken == null)
			{
				return null;
			}

			if (jToken is JObject || jToken is JArray)
			{
				return jToken.ToString(Formatting.None);
			}

			// if jToken is JValue or smth else
			return jToken.ToString();
		}

		public static JToken ApplyStringContent(this JToken destination, string content)
		{
			if (destination is JObject)
			{
				destination = JObject.Parse(content);
			}
			else if (destination is JArray)
			{
				destination = new JArray(content);
			}
			else
			{
				// destination is JValue
				destination = content;
			}

			return destination;
		}
	}
}
