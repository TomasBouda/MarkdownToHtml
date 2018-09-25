using HandlebarsDotNet;
using Markdig;
using Markdig.SyntaxHighlighting;
using System;
using System.IO;
using TomLabs.Shadowgem.Extensions.String;

namespace TomLabs.MDToHtml.Core
{
	public class HtmlPage
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

		private static string _template;

		private static string Template
		{
			get
			{
				if (!_template.IsFilled())
				{
					_template = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}pageTemplate.html");
				}
				return _template;
			}
		}

		public static string NavigationHtml { get; set; }

		/// <summary>
		/// Prop for handlebars
		/// </summary>
		public string Nav => NavigationHtml;

		private string MarkdownString { get; set; }

		public string InnerHtml { get; set; }

		public string PageTitle { get; set; }

		public string Html => Compile();

		public HtmlPage(string markdownString, string pageTitle)
		{
			MarkdownString = markdownString;
			InnerHtml = Markdown.ToHtml(MarkdownString, _pipeline);
			PageTitle = pageTitle;
		}

		protected virtual string Compile()
		{
			var hb = Handlebars.Compile(Template);
			return hb(this);
		}
	}
}