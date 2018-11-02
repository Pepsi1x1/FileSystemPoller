using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DT.FileSystem
{
	public partial class FileSystemPoller : DT.FileSystem.Interfaces.IFileSystemInfoProvider
	{
		private readonly List<string> _fileList;

		public HashSet<string> ChangedItems { get; } = new HashSet<string>();

		public static FileSystemPoller Instance { get; private set; }

		//public string ExcludeFolder { get; set; }

		public string WatchFolder { get; set; }

		public event EventHandler OnFilesReady;

		private readonly System.Timers.Timer _timer;

		private DateTime _lastChecked;

		private DT.FileSystem.Data.Implementations.WatchedFileRepository _context;
		
		private bool _firstRun;

		public FileSystemPoller(string watchFolder)
		{
			NLog.ILogger logger = NLog.LogManager.GetCurrentClassLogger();
		    DT.FileSystem.Data.Interfaces.ISQLExecuter sqlExecuter =
				new DT.FileSystem.Data.Implementations.SQLExecuter();

			_context = new Data.Implementations.WatchedFileRepository(logger, sqlExecuter);

			this.WatchFolder = watchFolder;

			this._timer = new System.Timers.Timer();

			//When autoreset is True there are reentrancy problems.
			this._timer.AutoReset = false;

			this._timer.Elapsed += EnumerateFileSystemChanges;

			this._fileList = _context.GetAll().Where(wf => !wf.Deleted).Select(w => w.Path).ToList();
			if (!this._fileList.Any())
				this._firstRun = true;
		}

		//[Initialization(InitializationPass.Second, "Setting up filesystem polling")]
		public static void Initialize(string watchFolder)
		{
			Instance = new FileSystemPoller(watchFolder);
		}


		private void EnumerateFileSystemChanges(object sender, System.Timers.ElapsedEventArgs e)
		{
			this._lastChecked = DateTime.Now;
			try
			{
				var files = GetAllFilesList();

				var previousFileList = new List<string>(this._fileList);

				foreach (var file in files.Where(file => !previousFileList.Contains(file)))
				{
					this._fileList.Add(file);
				}

				var removeList = this._fileList.Where(file => !files.Contains(file)).ToList();
				foreach (var fileToRemove in removeList)
				{
					this._fileList.Remove(fileToRemove);
					RemoveByFileName(fileToRemove);
				}

				lock (this.ChangedItems)
				{
					var differenceQuery = this._firstRun ? this._fileList : this._fileList.Except(previousFileList);
					this._firstRun = false;
					foreach (var difference in differenceQuery)
					{
						if (this.ChangedItems.Add(difference))
						{
							AddToDb(difference);
						}
						else
						{
							System.Diagnostics.Debug.WriteLine($"Duplicate watcher entry {difference}");
							System.Diagnostics.Trace.WriteLine($"Duplicate watcher entry {difference}");
						}
					}
					//fileList = tempFileList;
				}

				if (this.ChangedItems.Any())
					OnFilesReady?.Invoke(this, EventArgs.Empty);
			}
			finally
			{
				TimeSpan ts = DateTime.Now.Subtract(this._lastChecked);
				TimeSpan maxWaitTime = TimeSpan.FromMinutes(5);

				if (maxWaitTime.Subtract(ts).CompareTo(TimeSpan.Zero) > -1)
					this._timer.Interval = maxWaitTime.Subtract(ts).TotalMilliseconds;
				else
					this._timer.Interval = 1;

				this._timer.Start();
			}
		}

		private IEnumerable<string> GetAllFilesList()
		{
			IEnumerable<string> contents =
				Directory.GetFiles(this.WatchFolder, "*",
					SearchOption.TopDirectoryOnly);

			//if (!string.IsNullOrEmpty(this.ExcludeFile))
			//	contents = contents.Where(d => d != this.ExcludeFile);

			return contents;
		}

		private void AddRangeToDb(IEnumerable<string> items)
		{
			lock (this._fileList)
			{
				var files = items as IList<string> ?? items.ToList();

				foreach (var file in files)
				{
					_context.Create(new Data.WatchedFile() { Path = file });
				}
			}
		}

		public void AddToDb(string file)
		{
			_context.Create(new Data.WatchedFile() { Path = file });
		}

		public void RemoveByFileName(string file)
		{
			var removes = _context.GetAll(file);
			_context.RemoveRange(removes.ToList());
		}
	}
}
