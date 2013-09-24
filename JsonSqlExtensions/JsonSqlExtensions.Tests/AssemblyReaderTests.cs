using System.IO;
using JsonSqlExtensions.AssemblyHexExtractor;
using NUnit.Framework;

namespace JsonSqlExtensions.Tests
{
	[TestFixture]
	public class AssemblyReaderTests
	{
		private readonly AssemblyReader _assemblyReader ;

		public AssemblyReaderTests()
		{
			_assemblyReader = new AssemblyReader();
		}

		[TestCase(@"d:\Development\SqlExtensions\sql-extensions\JsonSqlExtensions\JsonSqlExtensions.Core\bin\Debug\JsonSqlExtensions.Core.dll")]
		[TestCase(@"d:\Development\SqlExtensions\sql-extensions\JsonSqlExtensions\JsonSqlExtensions.StrongNameSign\bin\Debug\JsonSqlExtensions.StrongNameSign.dll")]
		public void GetHexString(string assemblyPath)
		{
			var result = _assemblyReader.GetAssemblyHexString(assemblyPath);

			File.WriteAllText(string.Format("{0}.txt", Path.GetFileName(assemblyPath)), result);
		}
	}
}
