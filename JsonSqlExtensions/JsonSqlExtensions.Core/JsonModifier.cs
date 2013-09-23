using System;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json.Linq;

namespace JsonSqlExtensions.Core
{
	public class JsonModifier
	{
		[SqlFunction]
		public static string GetJsonProperty(string jsonString, string propertyFullKey)
		{
			JObject jsonObject = JObject.Parse(jsonString);

			var propertiesKeys = JsonPathParser.ParseFullJsonPropertyKey(propertyFullKey);

			JToken propertyValue = jsonObject;

			foreach (var propertyKey in propertiesKeys)
			{
				try
				{
					propertyValue = propertyValue[propertyKey];
				}
				catch (Exception)
				{
					return null;
				}

				if (propertyValue == null)
				{
					return null;
				}
			}

			return propertyValue.ToJsonString();
		}

		public static string SetJsonProperty(string initialJson, string propertyFullKey, string newValue)
		{
			var newFormatedValue = string.Format("{{\"value\":\"{0}\"}}", newValue);

			JToken newValueJsonObject = JObject.Parse(newFormatedValue)[Constants.Value];

			JObject jsonObject = JObject.Parse(initialJson);

			var propertiesKeys = JsonPathParser.ParseFullJsonPropertyKey(propertyFullKey);

			JToken jsonElement = jsonObject;

			for(int i = 0; i < propertiesKeys.Count; i++)
			{
				var propertyKey = propertiesKeys[i];

				jsonElement = jsonElement[propertyKey];
			}

			jsonElement = newValueJsonObject;

			return jsonObject.ToJsonString();
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

			return jsonObject.ToJsonString();
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

			var stringValue = jToken.ToJsonString();

			stringValue = stringValue.Replace(assignmentsBig2, responsiblePerson);

			jToken = jToken.ApplyStringContent(stringValue);

			return jToken;
		}
	}
}
