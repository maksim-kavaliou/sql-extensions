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
		[TestCase("{\"id\":[[\"1\",\"2\"],[]]}", "id.[0]", "[\"1\",\"2\"]")]
		[TestCase("{\"id\":[[\"1\",\"2\"],[]]}", "id.[1]", "[]")]
		[TestCase("{\"id\":[[\"1\",\"2\"],[]]}", "id.[0].[0]", "1")]
		[TestCase("{\"id\":[[\"1\",\"2\"],[]]}", "id.[0].[0].[0]", null)]
		[TestCase("{\"id\":\"null\"}", "id", "null")]
		[TestCase("{\"id\":null}", "id", "")]
		[TestCase("{\"id\": \"1\", \"name\": \"maks\", \"address\": {\"street\": \"xsxsxs\", \"building\": \"4\"} }", "id", "1")]
		[TestCase("{\"id\": \"1\", \"name\": \"maks\", \"address\": {\"street\": \"xsxsxs\", \"building\": \"4\"} }", "name", "maks")]
		[TestCase("{\"id\": \"1\", \"name\": \"maks\", \"address\": {\"street\": \"xsxsxs\", \"building\": \"4\"} }", "address.building", "4")]
		[TestCase("{\"id\": \"1\", \"name\": \"maks\", \"address\": {\"street\": \"xsxsxs\", \"building\": \"4\"} }", "address", "{\"street\":\"xsxsxs\",\"building\":\"4\"}")]
		public void JsonModifier_GetJsonProperty_Check(string jsonString, string propertyKey, string propertyAssertValue)
		{
			var propertyValue = JsonModifier.GetJsonProperty(jsonString, propertyKey);

			Assert.That(propertyValue, Is.EqualTo(propertyAssertValue));
		}

		[TestCase(null, "id.as.[3]", "s1234", "{\"id\":{\"as\":[null,null,null,\"s1234\"]}}")]
		[TestCase("", "id.as.[3].xx", "s1234", "{\"id\":{\"as\":[null,null,null,{\"xx\":\"s1234\"}]}}")]
		[TestCase(null, "id.as.[3]", "{\"x\":\"1234\"}", "{\"id\":{\"as\":[null,null,null,{\"x\":\"1234\"}]}}")]
		[TestCase(null, "id.as.[3]", "[\"x\",\"1234\"]", "{\"id\":{\"as\":[null,null,null,[\"x\",\"1234\"]]}}")]
		[TestCase(null, "id.as.[3]", @"[""x"",""1234""]zxcz", "{\"id\":{\"as\":[null,null,null,\"[\\\"x\\\",\\\"1234\\\"]zxcz\"]}}")]
		[TestCase("{\"id\":\"2\"}", "id", "3s", "{\"id\":\"3s\"}")]
		[TestCase("{\"id\":\"2\"}", "name", "Alex", "{\"id\":\"2\",\"name\":\"Alex\"}")]
		[TestCase("{\"id\":\"2\",\"name\":\"Alex\",\"address\":[{\"street\":\"XXX\"},{\"building\":\"22\"}]}", "address.[3].republic", "belarus", "{\"id\":\"2\",\"name\":\"Alex\",\"address\":[{\"street\":\"XXX\"},{\"building\":\"22\"},null,{\"republic\":\"belarus\"}]}")]
		[TestCase("{\"id\":\"2\",\"name\":\"Alex\",\"address\":[{\"street\":\"XXX\"},{\"building\":\"22\"}]}", "country.[3].republic", "belarus", "{\"id\":\"2\",\"name\":\"Alex\",\"address\":[{\"street\":\"XXX\"},{\"building\":\"22\"}],\"country\":[null,null,null,{\"republic\":\"belarus\"}]}")]
		public void SetJsonProperty_Check(string initialJson, string propertyFullKey, string newValue, string assertValue)
		{
			var result = JsonModifier.SetJsonProperty(initialJson, propertyFullKey, newValue);

			Assert.That(result, Is.EqualTo(assertValue));
		}

		[TestCase(
			"{\"impediment\":{\"l\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"tags_short\",\"alignment\":\"base\"},{\"id\":\"request_private\",\"alignment\":\"base\"},{\"id\":\"project_abbr\",\"alignment\":\"alt\"}],[{\"id\":\"entity_name_3lines\",\"alignment\":\"base\"}],[],[{\"id\":\"owner\",\"alignment\":\"alt\"},{\"id\":\"related_entity_short\",\"alignment\":\"alt\"}],[{\"id\":\"dependencies\",\"alignment\":\"base\"},{\"id\":\"business_value_short\",\"alignment\":\"base\"},{\"id\":\"assignments_big_2\",\"alignment\":\"base\"}]]},\"userstory\":{\"m\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"impediments\",\"alignment\":\"alt\"}],[],[{\"id\":\"create_date\",\"alignment\":\"alt\"},{\"id\":\"assignments_big_2\",\"alignment\":\"base\"}]]},\"bug\":{\"m\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"tags_short_xs\",\"alignment\":\"base\"},{\"id\":\"impediments\",\"alignment\":\"alt\"}],[],[{\"id\":\"effort_total\",\"alignment\":\"base\"}]]}}",
			"impediment",
			"assignments_big_2",
			"responsible_person",
			"{\"impediment\":{\"l\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"tags_short\",\"alignment\":\"base\"},{\"id\":\"request_private\",\"alignment\":\"base\"},{\"id\":\"project_abbr\",\"alignment\":\"alt\"}],[{\"id\":\"entity_name_3lines\",\"alignment\":\"base\"}],[],[{\"id\":\"owner\",\"alignment\":\"alt\"},{\"id\":\"related_entity_short\",\"alignment\":\"alt\"}],[{\"id\":\"dependencies\",\"alignment\":\"base\"},{\"id\":\"business_value_short\",\"alignment\":\"base\"},{\"id\":\"responsible_person\",\"alignment\":\"base\"}]]},\"userstory\":{\"m\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"impediments\",\"alignment\":\"alt\"}],[],[{\"id\":\"create_date\",\"alignment\":\"alt\"},{\"id\":\"assignments_big_2\",\"alignment\":\"base\"}]]},\"bug\":{\"m\":[[{\"id\":\"general_entity_id\",\"alignment\":\"base\"},{\"id\":\"tags_short_xs\",\"alignment\":\"base\"},{\"id\":\"impediments\",\"alignment\":\"alt\"}],[],[{\"id\":\"effort_total\",\"alignment\":\"base\"}]]}}"
		)]
		[TestCase(
			"{\"impediment\":\"assignments_big_2\"}",
			"impediment",
			"assignments_big_2",
			"responsible_person", 
			"{\"impediment\":\"responsible_person\"}"
		)]
		[TestCase(
			"{\"impediment\":[[\"11\",\"assignments_big_2\",[\"33\",\"44\"]],{\"id\":\"assignments_big_2\"},{\"id\":\"bla-bla-bla\"}]}",
			"impediment",
			"assignments_big_2",
			"responsible_person", 
			"{\"impediment\":[[\"11\",\"responsible_person\",[\"33\",\"44\"]],{\"id\":\"responsible_person\"},{\"id\":\"bla-bla-bla\"}]}"
		)]
		[TestCase("{\"impediment\":[]}",
			"impediment",
			"assignments_big_2",
			"responsible_person", 
			"{\"impediment\":[]}"
		)]
		public void ReplaceElementContent_Check(string jsonString, string propertyFullKey, string oldValue, string newValue, string assertValue)
		{
			var result = JsonModifier.ReplaceElementContent(jsonString, propertyFullKey, oldValue, newValue);

			Assert.That(result, Is.EqualTo(assertValue));
		}

		[TestCase("asdf[0]", 0)]
		[TestCase("assert.value[6]", 6)]
		[TestCase("asdf[]", null)]
		[TestCase("asdf", null)]
		public void ParseArrayIndex_Check(string value, int? assertValue)
		{
			var result = JsonPathParser.ParseArrayIndex(value);

			Assert.That(result, Is.EqualTo(assertValue));
		}
	}
}
