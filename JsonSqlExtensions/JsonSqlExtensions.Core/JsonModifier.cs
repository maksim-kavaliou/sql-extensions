using System;
using System.Collections.Generic;
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

			try
			{
				return (string)propertyValue;
			}
			catch (Exception e)
			{
				return null;
			}
		}
	}
}
