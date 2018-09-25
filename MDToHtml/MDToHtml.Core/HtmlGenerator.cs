using System.IO;
using System.Text;
using TomLabs.MDToHtml.Files;

namespace TomLabs.MDToHtml.Core
{
	public class HtmlGenerator
	{
		private FileTree Root { get; set; }

		public void Generate(string mdDirectoryPath, string newDirectoryPath = null)
		{
			Root = FileTree.GetTree(mdDirectoryPath, "*.md", new string[] { ".git", ".attachments" });
			HtmlPage.NavigationHtml = GenerateNavigation(Root, newDirectoryPath ?? $"{Root.Path}_html").ToString();
			Crawl(Root, newDirectoryPath ?? $"{Root.Path}_html");
		}

		public void Crawl(FileTree fileTree, string newDirectoryPath)
		{
			Directory.CreateDirectory(fileTree.Path.Replace(Root.Path, newDirectoryPath));

			if (fileTree.SubDirectories.Count > 0)  // Dive in
			{
				foreach (var subDir in fileTree.SubDirectories)
				{
					Crawl(subDir, newDirectoryPath);
				}
			}

			foreach (var file in fileTree.Files)
			{
				TransferFile(file.FullName, file.FullName.Replace(Root.Path, newDirectoryPath));
			}
		}

		private StringBuilder GenerateNavigation(FileTree fileTree, string newDirectoryPath, StringBuilder sb = null)
		{
			(sb ?? (sb = new StringBuilder())).Append("<ul>");

			if (fileTree.SubDirectories.Count > 0)  // Dive in
			{
				foreach (var subDir in fileTree.SubDirectories)
				{
					sb.Append($"<li>{subDir.Info.Name}");
					GenerateNavigation(subDir, newDirectoryPath, sb);
					sb.Append("</li>");
				}
			}

			foreach (var file in fileTree.Files)
			{
				sb.Append($"<li><a href='{file.FullName.Replace(Root.Path, "").Replace("\\", "/").Replace(".md", ".html")}'>{file.Name}</a></li>");
			}

			sb.Append("</ul>");

			return sb;
		}

		public void TransferFile(string filePath, string targetFilePath = null)
		{
			var md = File.ReadAllText(filePath, Encoding.UTF8);
			var page = new HtmlPage(md, Path.GetFileNameWithoutExtension(filePath));

			File.WriteAllText($"{Path.GetDirectoryName(targetFilePath ?? filePath)}\\{Path.GetFileNameWithoutExtension(targetFilePath ?? filePath)}.html", page.Html, Encoding.UTF8);
		}
	}
}