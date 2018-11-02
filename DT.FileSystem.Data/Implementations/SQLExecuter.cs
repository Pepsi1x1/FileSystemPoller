using Dapper;

namespace DT.FileSystem.Data.Implementations
{
	public class SQLExecuter : DT.FileSystem.Data.Interfaces.ISQLExecuter
	{
		public System.Collections.Generic.IEnumerable<TResponse> Query<TResponse>(System.Data.IDbConnection connection, string sql, object param, System.Data.CommandType commandType)
		{
			try
			{
				return connection.Query<TResponse>(sql: sql, param: param, commandType: commandType);
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				switch (ex.Number)
				{
					case (int)SQLExceptionCodes.ENTITY_NOT_FOUND:
						throw new DT.FileSystem.Data.Exceptions.EntityNotFoundException(ex.Message);
					case (int)SQLExceptionCodes.DUPLICATE_ENTITY:
						throw new DT.FileSystem.Data.Exceptions.DuplicateEntityException(ex.Message);
					default:
						throw;
				}
			}
		}

		public void Execute(System.Data.IDbConnection connection, string sql, object param, System.Data.CommandType commandType)
		{
			try
			{
				connection.Execute(sql: sql, param: param, commandType: commandType);
			}
			catch (System.Data.SqlClient.SqlException ex)
			{
				switch (ex.Number)
				{
					case (int)SQLExceptionCodes.ENTITY_NOT_FOUND:
						throw new DT.FileSystem.Data.Exceptions.EntityNotFoundException(ex.Message);
					case (int)SQLExceptionCodes.DUPLICATE_ENTITY:
						throw new DT.FileSystem.Data.Exceptions.DuplicateEntityException(ex.Message);
					default:
						throw;
				}
			}
		}
	}
}
