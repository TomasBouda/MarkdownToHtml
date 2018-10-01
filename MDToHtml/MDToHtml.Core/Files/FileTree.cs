using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TomLabs.MDToHtml.Files
{
	public class FileTree
	{
		public string Path { get; private set; }

		public DirectoryInfo Info { get; private set; }

		public List<FileTree> SubDirectories { get; private set; }

		public List<FileInfo> Files { get; private set; }

		public FileTree(string path, string fileRegexSearchPattern, string[] excludedDirectoryNames = null)
		{
			if (Directory.Exists(path))
			{
				Path = path;
				Info = new DirectoryInfo(Path);
				SubDirectories = Directory.GetDirectories(Path, "*")
					.Where(x => !excludedDirectoryNames?.Contains(System.IO.Path.GetFileName(x)) ?? true)
					.Select(s => new FileTree(s, fileRegexSearchPattern))
					.ToList();
				Files = Directory.GetFiles(Path, "*.*")
					.Where(f => Regex.IsMatch(f, fileRegexSearchPattern))
					.Select(s => new FileInfo(s))
					.ToList();
			}
		}

		public static FileTree GetTree(string path, string fileRegexSearchPattern = ".*\\..*", string[] excludedDirectoryNames = null)
		{
			return new FileTree(path, fileRegexSearchPattern, excludedDirectoryNames);
		}
	}
}