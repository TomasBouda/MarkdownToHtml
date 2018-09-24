using System.IO;
using System.Reflection;

namespace MDToHtml.Core.Utils
{
	public static class Embeded
	{
		public static string GetEmbededFile(string fileName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = $"{nameof(MDToHtml)}.{nameof(Core)}.{fileName}";

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}
	}
}