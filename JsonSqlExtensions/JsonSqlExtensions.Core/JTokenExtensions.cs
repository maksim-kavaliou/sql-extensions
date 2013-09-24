using System;
using System.Collections.Generic;
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

		public static JToken BuildInnerElement(this JToken initialToken, string fullKey, string value)
		{
			if (string.IsNullOrEmpty(fullKey))
			{
				return initialToken;
			}

			JToken lastTokenValue = JTokenBuilder.Build(value);

			var propertiesKeys = JsonPathParser.ParseFullJsonPropertyKey(fullKey);

			var linkedKeys = new LinkedList<object>(propertiesKeys);

			return initialToken.BuildInnerElement(linkedKeys.First, lastTokenValue);
		}

		public static JToken BuildInnerElement(this JToken initialToken, LinkedListNode<object> key, JToken lastTokenValue)
		{
			JToken innerToken;

			try
			{
				innerToken = initialToken[key.Value];
			}
			catch (Exception)
			{
				// build JObject
				if (key.Value is string)
				{
					var property = new JProperty((string)key.Value, null);

					if (initialToken is JObject)
					{
						((JObject)initialToken).Add(property);
					}
					else
					{
						initialToken = new JObject(property);
					}

				}
				else if (key.Value is int?)
				{
					// build JArray

					if (initialToken is JArray)
					{
						while (((JArray)initialToken).Count <= (int)(key.Value))
						{
							((JArray)initialToken).Add(null);
						}
					}
					else
					{
						var index = (int)(key.Value);
						var elements = new object[index + 1];
						initialToken = new JArray(elements);
					}
				}

				// now everythink must be ok
				innerToken = initialToken[key.Value];
			}


			if (key.Next != null)
			{
				initialToken[key.Value] = innerToken.BuildInnerElement(key.Next, lastTokenValue);
			}
			else
			{
				initialToken[key.Value] = lastTokenValue;
			}

			return initialToken;
		}
	}
}
