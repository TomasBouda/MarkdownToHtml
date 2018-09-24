using Markdig;
using MDToHtml.Core.Utils;
using System.IO;
using System.Text;
using TomLabs.Shadowgem.Extensions.String;

namespace MDToHtml.Core
{
	public class HtmlGenerator
	{
		public static void Generate()
		{
			var pipeline = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.UseEmojiAndSmiley()
				.UseSoftlineBreakAsHardlineBreak()
				.UseBootstrap()
				.Build();

			var md = File.ReadAllText(@"C:\Data\WORK\FairCredit.wiki\Dokumentace\Certifikace.md", Encoding.UTF8);
			var result = Markdown.ToHtml(md, pipeline);

			var usings = Embeded.GetEmbededFile("templateUsings.html");
			var html = Embeded.GetEmbededFile("pageTemplate.html").FillIn(result, usings);
			File.WriteAllText(@"C:\Data\WORK\FairCredit.wiki\Dokumentace\Certifikace.html", html, Encoding.UTF8);
		}
	}
}