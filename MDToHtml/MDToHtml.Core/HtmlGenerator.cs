﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using TomLabs.MDToHtml.Files;

namespace TomLabs.MDToHtml.Core
{
	public class HtmlGenerator
	{
		private FileTree SourceTree { get; set; }

		public void Generate(string mdDirectoryPath, string newDirectoryPath = null, string attachementsPath = null)
		{
			SourceTree = FileTree.GetTree(mdDirectoryPath, ".*\\.md|\\.order", new string[] { ".git", ".attachments" });
			string newDir = newDirectoryPath ?? $"{SourceTree.Path}_html";

			Console.WriteLine($"Generating html wiki from {mdDirectoryPath} into {newDir}");

			Crawl(SourceTree, newDir);

			Console.WriteLine($"Generating navigation and index page");
			string homeHtml = File.Exists($"{newDir}\\Home.html") ? File.ReadAllText($"{newDir}\\Home.html") : File.ReadAllText(Directory.GetFiles(newDir).FirstOrDefault(f => !f.StartsWith("index")));
			File.WriteAllText($"{newDir}\\index.html", new Index(GenerateNavigation(SourceTree, newDir).ToString(), "Wiki", homeHtml).Html);

			var attachmementsSource = attachementsPath ?? $"{mdDirectoryPath}\\.attachments";
			var attachmementsTarget = $"{newDir}\\.attachments";

			CopyFilesRecursively(new DirectoryInfo(attachmementsSource), new DirectoryInfo(attachmementsTarget));
		}

		public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
		{
			if (!Directory.Exists(target.FullName))
			{
				Directory.CreateDirectory(target.FullName);
			}

			foreach (DirectoryInfo dir in source.GetDirectories())
			{
				CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
			}

			foreach (FileInfo file in source.GetFiles())
			{
				file.CopyTo(Path.Combine(target.FullName, file.Name), true);
			}
		}

		public void TransformFile(string filePath, string targetFilePath = null)
		{
			Console.WriteLine($"Processing file {filePath}");

			var md = File.ReadAllText(filePath, Encoding.UTF8);
			md = md.Replace("[[_TOC_]]", ""); // TODO

			var page = new MarkdownPage(md, Path.GetFileNameWithoutExtension(filePath));

			File.WriteAllText($"{Path.GetDirectoryName(targetFilePath ?? filePath)}\\{Path.GetFileNameWithoutExtension(targetFilePath ?? filePath)}.html", page.Html, Encoding.UTF8);
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

			foreach (var file in fileTree.Files.Where(f => !f.Name.EndsWith(".order")))
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
					string fileRelPath = subDir..FullName.Replace(SourceTree.Path, "").Replace("\\", "/").Replace(".md", ".html");
					sb.Append($"<li><aa class='link' data-file-path='{subDir.Path}.md'>{subDir.Info.Name.Replace("-", " ")}</aa>");
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