using System;
using System.Collections.Generic;
using System.Text;

namespace DT.FileSystem.Interfaces
{
	public interface IFileSystemInfoProvider
	{
		HashSet<string> ChangedItems { get; }

		event EventHandler OnFilesReady;
	}
}
