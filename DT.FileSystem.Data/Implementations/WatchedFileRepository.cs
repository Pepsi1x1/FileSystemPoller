using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DT.FileSystem.Data.Implementations
{
	public class WatchedFileRepository
	{
		private const string CONNECTION_STRING_KEY = "FileSystemDatabase";
		private const string CREATE_WATCHED_FILE_SP = "[FS].[Create]";
		private const string DELETE_WATCHED_FILE_SP = "[FS].[Delete]";
		private const string GETALL_WATCHED_FILE_SP = "[FS].[RetrieveAll]";
		private const string GETALL_BYNAME_WATCHED_FILE_SP = "[FS].[RetrieveAllByName]";
		private readonly string _connectionString;
		private readonly NLog.ILogger _logger;
		private readonly DT.FileSystem.Data.Interfaces.ISQLExecuter _sqlExecuter;

		public WatchedFileRepository(NLog.ILogger logger, DT.FileSystem.Data.Interfaces.ISQLExecuter sqlExecuter)
		{
			this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
			this._connectionString = System.Configuration.ConfigurationManager.AppSettings[CONNECTION_STRING_KEY];
			this._sqlExecuter = sqlExecuter;
		}

		public WatchedFile Create(WatchedFile watchedFile)
		{
			if (watchedFile == null)
			{
				throw new ArgumentNullException(nameof(watchedFile));
			}
			else if (string.IsNullOrWhiteSpace(watchedFile.Path))
			{
				throw new ArgumentNullException(nameof(watchedFile.Path));
			}
			else
			{
				this._logger.Info($"Executing {CREATE_WATCHED_FILE_SP}");
				using (System.Data.IDbConnection dbConnection = new System.Data.SqlClient.SqlConnection(this._connectionString))
				{
					return _sqlExecuter.Query<WatchedFile>(dbConnection, sql: CREATE_WATCHED_FILE_SP,
						param: new { path = watchedFile.Path }, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
				}
			}
		}

		public IEnumerable<WatchedFile> GetAll()
		{
			this._logger.Info($"Executing {GETALL_WATCHED_FILE_SP}");
			using (System.Data.IDbConnection dbConnection =
				new System.Data.SqlClient.SqlConnection(this._connectionString))
			{
				return _sqlExecuter.Query<WatchedFile>(dbConnection, sql: GETALL_WATCHED_FILE_SP,
					param: null, commandType: System.Data.CommandType.StoredProcedure);
			}
		}

		public IEnumerable<WatchedFile> GetAll(string watchedFilePath)
		{
			this._logger.Info($"Executing {GETALL_BYNAME_WATCHED_FILE_SP}");
			using (System.Data.IDbConnection dbConnection =
				new System.Data.SqlClient.SqlConnection(this._connectionString))
			{
				return _sqlExecuter.Query<WatchedFile>(dbConnection, sql: GETALL_BYNAME_WATCHED_FILE_SP,
					param: new { path = watchedFilePath }, commandType: System.Data.CommandType.StoredProcedure);
			}
		}

		public void Delete(WatchedFile watchedFile)
		{
			if (watchedFile == null)
			{
				throw new ArgumentNullException(nameof(watchedFile));
			}
			else if(watchedFile.Id == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(watchedFile.Id));
			}
			else{
				this._logger.Info($"Executing {DELETE_WATCHED_FILE_SP}");
				using (System.Data.IDbConnection dbConnection = new System.Data.SqlClient.SqlConnection(this._connectionString))
				{
					_sqlExecuter.Execute(dbConnection, sql: DELETE_WATCHED_FILE_SP,
						param: new { id = watchedFile.Id }, commandType: System.Data.CommandType.StoredProcedure);
				}
			}
		}

		public void RemoveRange(List<WatchedFile> toList)
		{
			if (toList == null)
			{
				throw new ArgumentNullException(nameof(toList));
			}
			else if (toList.Count == 0)
			{
				throw new ArgumentOutOfRangeException(nameof(toList));
			}
			else
			{
				//Todo: TVP
				this._logger.Info($"Executing {DELETE_WATCHED_FILE_SP}");
				using (System.Data.IDbConnection dbConnection = new System.Data.SqlClient.SqlConnection(this._connectionString))
				{
					_sqlExecuter.Execute(dbConnection, sql: DELETE_WATCHED_FILE_SP,
						param: new { files = toList }, commandType: System.Data.CommandType.StoredProcedure);
				}
			}
		}
	}
}
