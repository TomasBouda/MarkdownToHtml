using HandlebarsDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TomLabs.Shadowgem.Extensions.String;

namespace TomLabs.MDToHtml.Core
{
	public class Index
	{
		private static string _template;

		private static string Template
		{
			get
			{
				if (!_template.IsFilled())
				{
					_template = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}Content\\pageTemplate.html");
				}
				return _template;
			}
		}

		public string Nav { get; set; }

		public string PageContent { get; set; }

		public string PageTitle { get; set; }

		public string Html => Compile();

		public List<CssTemplate> CssIncludes { get; set; }
		public List<JsTemplate> JsIncludes { get; set; }

		public Index()
		{
			Handlebars.RegisterTemplate(nameof(CssTemplate.CssContent), File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}Content\\cssTemplate.html"));
			Handlebars.RegisterTemplate(nameof(JsTemplate.JsContent), File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}Content\\jsTemplate.html"));
		}

		public Index(string navigation, string pageTitle, string pageContent = "") : this()
		{
			Nav = navigation;
			PageTitle = pageTitle;
			PageContent = pageContent;

			CssIncludes = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}Content\\css", "*.css").Select(f => new CssTemplate { CssContent = File.ReadAllText(f, Encoding.UTF8) }).ToList();
			JsIncludes = Directory.GetFiles($"{AppDomain.CurrentDomain.BaseDirectory}Content\\js", "*.js").Select(f => new JsTemplate { JsContent = File.ReadAllText(f, Encoding.UTF8) }).ToList();
		}

		protected virtual string Compile()
		{
			var hb = Handlebars.Compile(Template);
			return hb(this);
		}
	}
}