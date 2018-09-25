using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TomLabs.MDToHtml.Storage
{
	public class FileTree
	{
		public string Path { get; private set; }

		public DirectoryInfo Info { get; private set; }

		public List<FileTree> SubDirectories { get; private set; }

		public List<FileInfo> Files { get; private set; }

		public FileTree(string path, string fileSearchPattern)
		{
			if (Directory.Exists(path))
			{
				Path = path;
				Info = new DirectoryInfo(Path);
				SubDirectories = Directory.GetDirectories(Path, "*")
					.Select(s => new FileTree(s, fileSearchPattern))
					.ToList();
				Files = Directory.GetFiles(Path, fileSearchPattern)
					.Select(s => new FileInfo(s))
					.ToList();
			}
		}

		public static FileTree GetTree(string path, string fileSearchPattern = "*.*")
		{
			return new FileTree(path, fileSearchPattern);
		}
	}
}