using CommandLine;
using System.IO;
using TomLabs.MDToHtml.Core;
using static System.Console;

namespace TomLabs.MDToHtml.Console
{
	internal class Program
	{
		public class Options
		{
			[Option('i', "input", Required = true, HelpText = "Path to a markdown wiki")]
			public string Input { get; set; }

			[Option('o', "output", Required = false, HelpText = "Path where html wiki will be created")]
			public string Output { get; set; } = null;

			[Option('a', "attachements", Required = false, HelpText = "Path to source .attachements directory")]
			public string AttachementsPath { get; set; } = null;
		}

		private static void Main(string[] args)
		{
			var hg = new HtmlGenerator();
			Parser.Default.ParseArguments<Options>(args)
				.WithParsed<Options>(o =>
				{
					if (Directory.Exists(o.Input))
					{
						hg.Generate(o.Input, o.Output, o.AttachementsPath);
						WriteLine($"All done");
					}
					else
					{
						WriteLine($"Directory {o.Input} not found!");
					}
				});
		}
	}
}