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
			JToken jsonObject = JTokenBuilder.Build(jsonString);

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

		[SqlFunction]
		public static string SetJsonProperty(string initialJson, string propertyFullKey, string newValue)
		{
			if (string.IsNullOrEmpty(propertyFullKey))
			{
				return initialJson;
			}

			JToken jsonObject = JTokenBuilder.Build(initialJson);

			JToken result = jsonObject.BuildInnerElement(propertyFullKey, newValue);

			return result.ToJsonString();
		}

		[SqlFunction]
		public static string ReplaceElementContent(string initialJson, string propertyFullKey, string oldValue, string newValue)
		{
			if (string.IsNullOrEmpty(oldValue) || newValue == null)
			{
				return initialJson;
			}

			var elementValue = GetJsonProperty(initialJson, propertyFullKey);

			if (string.IsNullOrEmpty(elementValue))
			{
				return initialJson;
			}

			var newElementValue = elementValue.Replace(oldValue, newValue);

			var result = SetJsonProperty(initialJson, propertyFullKey, newElementValue);

			return result;
		}
	}
}
