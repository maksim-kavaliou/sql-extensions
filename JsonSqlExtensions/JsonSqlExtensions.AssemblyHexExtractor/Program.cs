using System.Collections.Generic;
using System.IO;

namespace JsonSqlExtensions.AssemblyHexExtractor
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var assemblyReader = new AssemblyReader();

			var resultList = new List<string>();

			foreach (var assemblyPath in args)
			{
				var assemblyHexString = assemblyReader.GetAssemblyHexString(assemblyPath);

				resultList.Add(string.Format("{0} - {1}", assemblyPath, assemblyHexString));
			}

			File.WriteAllLines("AssembliesHex.txt", resultList.ToArray());
		}
	}
}
