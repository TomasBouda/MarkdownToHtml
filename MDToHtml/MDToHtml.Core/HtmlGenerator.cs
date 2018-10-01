using System.IO;
using System.Linq;
using System.Text;
using TomLabs.MDToHtml.Files;

namespace TomLabs.MDToHtml.Core
{
	public class HtmlGenerator
	{
		private FileTree SourceTree { get; set; }

		public void Generate(string mdDirectoryPath, string newDirectoryPath = null)
		{
			SourceTree = FileTree.GetTree(mdDirectoryPath, ".*\\.md|\\.order", new string[] { ".git", ".attachments" });
			string newDir = newDirectoryPath ?? $"{SourceTree.Path}_html";

			HtmlPage.NavigationHtml = GenerateNavigation(SourceTree, newDir).ToString();

			Crawl(SourceTree, newDir);

			string homeHtml = File.Exists($"{newDir}\\Home.html") ? File.ReadAllText($"{newDir}\\Home.html") : "";
			File.WriteAllText($"{newDir}\\index.html", new HtmlPage(homeHtml, "Wiki").Html);
		}

		public void TransformFile(string filePath, string targetFilePath = null)
		{
			var md = File.ReadAllText(filePath, Encoding.UTF8);
			var page = new HtmlPage(md, Path.GetFileNameWithoutExtension(filePath));

			File.WriteAllText($"{Path.GetDirectoryName(targetFilePath ?? filePath)}\\{Path.GetFileNameWithoutExtension(targetFilePath ?? filePath)}.html", page.InnerHtml, Encoding.UTF8);
		}

		private void Crawl(FileTree fileTree, string newDirectoryPath)
		{
			Directory.CreateDirectory(fileTree.Path.Replace(SourceTree.Path, newDirectoryPath));

			if (fileTree.SubDirectories.Count > 0)  // Dive in
			{
				foreach (var subDir in fileTree.SubDirectories)
				{
					Crawl(subDir, newDirectoryPath);
				}
			}

			foreach (var file in fileTree.Files)
			{
				TransformFile(file.FullName, file.FullName.Replace(SourceTree.Path, newDirectoryPath));
			}
		}

		private StringBuilder GenerateNavigation(FileTree fileTree, string newDirectoryPath, StringBuilder sb = null)
		{
			(sb ?? (sb = new StringBuilder())).Append("<ul>");

			if (fileTree.SubDirectories.Count > 0)  // Dive in
			{
				foreach (var subDir in fileTree.SubDirectories)
				{
					sb.Append($"<li>{subDir.Info.Name.Replace("-", " ")}");
					GenerateNavigation(subDir, newDirectoryPath, sb);
					sb.Append("</li>");
				}
			}

			var order = fileTree.Files.FirstOrDefault(f => f.Name == ".order");
			var orderedFilesList = fileTree.Files;
			if (order != null)
			{
				orderedFilesList.Remove(order);
				var orderList = File.ReadAllLines(order.FullName).ToList();
				orderedFilesList = orderedFilesList.OrderBy(f => orderList.IndexOf(f.Name.Replace(".md", ""))).ToList();
			}

			foreach (var file in orderedFilesList)
			{
				string fileRelPath = file.FullName.Replace(SourceTree.Path, "").Replace("\\", "/").Replace(".md", ".html");
				sb.Append($"<li><aa class='link' data-file-path='{fileRelPath}'>{Path.GetFileNameWithoutExtension(file.Name).Replace("-", " ")}</aa></li>");
			}

			sb.Append("</ul>");

			return sb;
		}
	}
}