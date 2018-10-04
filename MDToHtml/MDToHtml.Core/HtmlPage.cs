using Markdig;
using Markdig.SyntaxHighlighting;
using System;
using System.Text.RegularExpressions;

namespace TomLabs.MDToHtml.Core
{
	public class MarkdownPage
	{
		private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
				.UseAdvancedExtensions()
				.UseEmojiAndSmiley()
				.UseSoftlineBreakAsHardlineBreak()
				.UseBootstrap()
				.UsePipeTables()
				.UseGridTables()
				.UseSyntaxHighlighting()
				.Build();

		private string MarkdownString { get; }

		public string Html { get; set; }

		public string PageTitle { get; set; }

		public MarkdownPage(string markdownString, string pageTitle)
		{
			MarkdownString = markdownString;
			Html = Markdown.ToHtml(MarkdownString, _pipeline);

			if (!Html.StartsWith("<h1"))
			{
				Html = $"<h1>{pageTitle}</h1>{Environment.NewLine}{Environment.NewLine}" + Html;
			}

			Html = Regex.Replace(Html, "<a href=\"/(.*?)(\\.md)?\">", "<a class='link' data-file-path='$1.html' href='#'>");
			Html = Regex.Replace(Html, "\"\\.attachments", "\"/.attachments");

			PageTitle = pageTitle;
		}
	}
}