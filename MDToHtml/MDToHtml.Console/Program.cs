using TomLabs.MDToHtml.Core;

namespace TomLabs.MDToHtml.Console
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			var hg = new HtmlGenerator();
			hg.Generate(@"C:\Data\WORK\FairCredit.wiki");
		}
	}
}