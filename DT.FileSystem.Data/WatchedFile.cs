using System;
using System.Collections.Generic;
using System.Text;

namespace DT.FileSystem.Data
{
	public class WatchedFile
	{
		public int Id { get; set; }

		public string Path { get; set; }

		public bool Deleted { get; set; }
	}
}
