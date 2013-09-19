using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace JsonSqlExtensions.AssemblyHexExtractor
{
	public class AssemblyReader
	{
		public string GetAssemblyHexString(string assemblyPath)
		{
			if (!File.Exists(assemblyPath))
			{
				return string.Format("File \"{0}\" doesn't exist", assemblyPath);
			}

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

			return builder.ToString();
		}
	}
}
