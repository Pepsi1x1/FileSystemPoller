namespace DT.FileSystem.Data.Interfaces
{
	public interface ISQLExecuter
	{
		void Execute(System.Data.IDbConnection connection, string sql, object param, System.Data.CommandType commandType);
		System.Collections.Generic.IEnumerable<TResponse> Query<TResponse>(System.Data.IDbConnection connection, string sql, object param, System.Data.CommandType commandType);
	}
}