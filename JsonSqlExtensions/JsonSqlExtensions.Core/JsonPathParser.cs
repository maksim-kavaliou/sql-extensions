using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsonSqlExtensions.Core
{
	public static class JsonPathParser
	{
		public static List<object> ParseFullJsonPropertyKey(string propertyFullKey)
		{
			var result = new List<object>();

			var propertiesKeys = propertyFullKey.Split('.').ToList();

			foreach (var propertyKey in propertiesKeys)
			{
				var arrayIndex = ParseArrayIndex(propertyKey);

				if (arrayIndex == null)
				{
					result.Add(propertyKey);
				}
				else
				{
					result.Add(arrayIndex);
				}
			}

			return result;
		}

		public static int? ParseArrayIndex(string value)
		{
			var indexRegex = new Regex(Constants.ArrayIndexTemplate);

			Match indexMatch = indexRegex.Match(value);

			if (indexMatch.Success)
			{
				int result = int.Parse(indexMatch.Groups[Constants.Index].Value);

				return result;
			}

			return null;
		}
	}
}
