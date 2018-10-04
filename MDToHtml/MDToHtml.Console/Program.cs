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
		}

		private static void Main(string[] args)
		{
			var hg = new HtmlGenerator();
			Parser.Default.ParseArguments<Options>(args)
				.WithParsed<Options>(o =>
				{
					if (Directory.Exists(o.Input))
					{
						hg.Generate(o.Input, o.Output);
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