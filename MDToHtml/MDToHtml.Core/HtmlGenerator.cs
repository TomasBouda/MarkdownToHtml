using Markdig;
using Markdig.SyntaxHighlighting;
using System.IO;
using System.Text;
using TomLabs.MDToHtml.Core.Utils;
using TomLabs.MDToHtml.Storage;
using TomLabs.Shadowgem.Extensions.String;

namespace TomLabs.MDToHtml.Core
{
	public class HtmlGenerator
	{
		private MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.UseEmojiAndSmiley()
				.UseSoftlineBreakAsHardlineBreak()
				.UseBootstrap()
				.UsePipeTables()
				.UseGridTables()
				.UseSyntaxHighlighting()
				.Build();

		private FileTree Root { get; set; }
		private string NavigationHtml { get; set; }

		public void Generate(string filePath)
		{
			Root = FileTree.GetTree(filePath, "*.md");
			NavigationHtml = GenerateNavigation(Root).ToString();
			Crawl(Root);
		}

		public void Crawl(FileTree fileTree)
		{
			if (fileTree.SubDirectories.Count > 0)  // Dive in
			{
				foreach (var subDir in fileTree.SubDirectories)
				{
					Crawl(subDir);
				}
			}

			foreach (var file in fileTree.Files)
			{
				TransferFile(file.FullName);
			}
		}

		private StringBuilder GenerateNavigation(FileTree fileTree, StringBuilder sb = null)
		{
			(sb ?? (sb = new StringBuilder())).Append("<ul>");

			if (fileTree.SubDirectories.Count > 0)  // Dive in
			{
				foreach (var subDir in fileTree.SubDirectories)
				{
					sb.Append($"<li>{subDir.Info.Name}");
					GenerateNavigation(subDir, sb);
					sb.Append("</li>");
				}
			}

			foreach (var file in fileTree.Files)
			{
				sb.Append($"<li><a href='{file.FullName.Replace(".md", ".html")}'>{file.Name}</a></li>");
			}

			sb.Append("</ul>");

			return sb;
		}

		public void TransferFile(string filePath, string targetFilePath = null)
		{
			var md = File.ReadAllText(filePath, Encoding.UTF8);
			var result = Markdown.ToHtml(md, _pipeline);

			var usings = Embeded.GetEmbededFile("templateUsings.html");
			var html = Embeded.GetEmbededFile("pageTemplate.html").FillIn(result, usings, NavigationHtml);

			File.WriteAllText($"{Path.GetDirectoryName(targetFilePath ?? filePath)}\\{Path.GetFileNameWithoutExtension(targetFilePath ?? filePath)}.html", html, Encoding.UTF8);
		}
	}
}