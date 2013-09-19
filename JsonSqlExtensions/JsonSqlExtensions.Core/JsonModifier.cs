using System;
using System.Linq;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonSqlExtensions.Core
{
	public class JsonModifier
	{
		[SqlFunction]
		public static string GetJsonProperty(string jsonString, string propertyFullKey)
		{
			JObject jsonObject = JObject.Parse(jsonString);

			var propertiesKeys = propertyFullKey.Split('.').ToList();

			JToken propertyValue = jsonObject[propertiesKeys[0]];

			for (int i = 1; i < propertiesKeys.Count; i++)
			{
				propertyValue = propertyValue[propertiesKeys[i]];

				if (propertyValue == null)
				{
					return null;
				}
			}

			return JTokenToString(propertyValue);
		}

		[SqlFunction]
		public static string ModifyImpedimentCardSettings(string jsonString)
		{
			const string impediment = "impediment";
			
			JObject jsonObject = JObject.Parse(jsonString);

			JToken impedimentValue = jsonObject[impediment];

			if (impedimentValue != null)
			{
				if (impedimentValue is JArray)
				{
					ParseJArray(impedimentValue, TransferAssignmentsBig2ToResponsiblePerson);
				}
				else
				{
					impedimentValue = TransferAssignmentsBig2ToResponsiblePerson(impedimentValue);
				}

				jsonObject[impediment] = impedimentValue;
			}

			return JTokenToString(jsonObject);
		}

		public static string JTokenToString(JToken jToken)
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

		public static JToken ApplyStringToJToken(string content, JToken destination)
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

		public static void ParseJArray(JToken jToken, Func<JToken, JToken> jTokenHanler)
		{
			if (jToken == null)
			{
				return;
			}

			var array = (JArray) jToken;

			for(int i = 0; i < array.Count; i++)
			{
				var item = array[i];

				if (item is JArray)
				{
					ParseJArray(item, jTokenHanler);
				}
				else
				{
					array[i] = jTokenHanler(item);
				}
			}
		}

		public static JToken TransferAssignmentsBig2ToResponsiblePerson(JToken jToken)
		{
			const string assignmentsBig2 = "assignments_big_2";
			const string responsiblePerson = "responsible_person";

			var stringValue = JTokenToString(jToken);

			stringValue = stringValue.Replace(assignmentsBig2, responsiblePerson);

			jToken = ApplyStringToJToken(stringValue, jToken);

			return jToken;
		}
	}
}
