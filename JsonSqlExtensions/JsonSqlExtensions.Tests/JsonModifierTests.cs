using System;
using System.Globalization;
using System.IO;
using System.Text;
using JsonSqlExtensions.Core;
using NUnit.Framework;

namespace JsonSqlExtensions.Tests
{
	[TestFixture]
	public class JsonModifierTests
	{
		[TestCase("{id: \"1\", name: \"maks\", address: {street: \"xsxsxs\", building: \"4\"} }", "id", "1")]
		[TestCase("{id: \"1\", name: \"maks\", address: {street: \"xsxsxs\", building: \"4\"} }", "name", "maks")]
		[TestCase("{id: \"1\", name: \"maks\", address: {street: \"xsxsxs\", building: \"4\"} }", "address.building", "4")]
		[TestCase("{id: \"1\", name: \"maks\", address: {street: \"xsxsxs\", building: \"4\"} }", "address", null)]
		public void JsonModifier_ChangeProperty_Check(string jsonString, string propertyKey, string propertyAssertValue)
		{
			var propertyValue = JsonModifier.GetJsonProperty(jsonString, propertyKey);

			Assert.That(propertyValue, Is.EqualTo(propertyAssertValue));
		}

		[TestCase(@"d:\Development\SqlExtensions\SqlManagedExtensions\SqlManagedExtensions\bin\Debug\SqlManagedExtensions.dll")]
		public static void GetHexString(string assemblyPath)
		{
			if (!Path.IsPathRooted(assemblyPath))
				assemblyPath = Path.Combine(Environment.CurrentDirectory, assemblyPath);

			var builder = new StringBuilder();
			builder.Append("0x");

			using (var stream = new FileStream(assemblyPath, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				int currentByte = stream.ReadByte();
				while (currentByte > -1)
				{
					builder.Append(currentByte.ToString("X2", CultureInfo.InvariantCulture));
					currentByte = stream.ReadByte();
				}
			}

			var result = builder.ToString();

			Assert.That(result, Is.Not.Null);
		}
	}
}
