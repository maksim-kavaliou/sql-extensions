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
		[TestCase("{\"id\": \"1\", \"name\": \"maks\", \"address\": {\"street\": \"xsxsxs\", \"building\": \"4\"} }", "id", "1")]
		[TestCase("{\"id\": \"1\", \"name\": \"maks\", \"address\": {\"street\": \"xsxsxs\", \"building\": \"4\"} }", "name", "maks")]
		[TestCase("{\"id\": \"1\", \"name\": \"maks\", \"address\": {\"street\": \"xsxsxs\", \"building\": \"4\"} }", "address.building", "4")]
		[TestCase("{\"id\": \"1\", \"name\": \"maks\", \"address\": {\"street\": \"xsxsxs\", \"building\": \"4\"} }", "address", "{\"street\":\"xsxsxs\",\"building\":\"4\"}")]
		public void JsonModifier_ChangeProperty_Check(string jsonString, string propertyKey, string propertyAssertValue)
		{
			var propertyValue = JsonModifier.GetJsonProperty(jsonString, propertyKey);

			Assert.That(propertyValue, Is.EqualTo(propertyAssertValue));
		}

		[TestCase("{\"impediment\":{\"l\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"tags_short\",\"alignment\":\"base\"},{\"id\":\"request_private\",\"alignment\":\"base\"},{\"id\":\"project_abbr\",\"alignment\":\"alt\"}],[{\"id\":\"entity_name_3lines\",\"alignment\":\"base\"}],[],[{\"id\":\"owner\",\"alignment\":\"alt\"},{\"id\":\"related_entity_short\",\"alignment\":\"alt\"}],[{\"id\":\"dependencies\",\"alignment\":\"base\"},{\"id\":\"business_value_short\",\"alignment\":\"base\"},{\"id\":\"assignments_big_2\",\"alignment\":\"base\"}]]},\"userstory\":{\"m\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"impediments\",\"alignment\":\"alt\"}],[],[{\"id\":\"create_date\",\"alignment\":\"alt\"},{\"id\":\"assignments_big_2\",\"alignment\":\"base\"}]]},\"bug\":{\"m\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"tags_short_xs\",\"alignment\":\"base\"},{\"id\":\"impediments\",\"alignment\":\"alt\"}],[],[{\"id\":\"effort_total\",\"alignment\":\"base\"}]]}}", "{\"impediment\":{\"l\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"tags_short\",\"alignment\":\"base\"},{\"id\":\"request_private\",\"alignment\":\"base\"},{\"id\":\"project_abbr\",\"alignment\":\"alt\"}],[{\"id\":\"entity_name_3lines\",\"alignment\":\"base\"}],[],[{\"id\":\"owner\",\"alignment\":\"alt\"},{\"id\":\"related_entity_short\",\"alignment\":\"alt\"}],[{\"id\":\"dependencies\",\"alignment\":\"base\"},{\"id\":\"business_value_short\",\"alignment\":\"base\"},{\"id\":\"responsible_person\",\"alignment\":\"base\"}]]},\"userstory\":{\"m\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"impediments\",\"alignment\":\"alt\"}],[],[{\"id\":\"create_date\",\"alignment\":\"alt\"},{\"id\":\"assignments_big_2\",\"alignment\":\"base\"}]]},\"bug\":{\"m\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"tags_short_xs\",\"alignment\":\"base\"},{\"id\":\"impediments\",\"alignment\":\"alt\"}],[],[{\"id\":\"effort_total\",\"alignment\":\"base\"}]]}}")]
		[TestCase("{\"impediment\":\"assignments_big_2\"}", "{\"impediment\":\"responsible_person\"}")]
		[TestCase("{\"impediment\":[[\"11\",\"assignments_big_2\",[\"33\",\"44\"]],{\"id\":\"assignments_big_2\"},{\"id\":\"bla-bla-bla\"}]}", "{\"impediment\":[[\"11\",\"responsible_person\",[\"33\",\"44\"]],{\"id\":\"responsible_person\"},{\"id\":\"bla-bla-bla\"}]}")]
		[TestCase("{\"impediment\":[]}", "{\"impediment\":[]}")]
		public void ModifyCardSettingsCheck(string jsonString, string assertValue)
		{
			var result = JsonModifier.ModifyCardSettings(jsonString);

			Assert.That(result, Is.EqualTo(assertValue));
		}

		[TestCase(@"d:\Development\SqlExtensions\sql-extensions\JsonSqlExtensions\JsonSqlExtensions.Core\bin\Debug\JsonSqlExtensions.Core.dll")]
		[TestCase(@"d:\Development\SqlExtensions\sql-extensions\JsonSqlExtensions\JsonSqlExtensions.StrongNameSign\bin\Debug\JsonSqlExtensions.StrongNameSign.dll")]
		public void GetHexString(string assemblyPath)
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
