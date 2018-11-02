CREATE PROC [FS].[RetrieveAllByName] 
    @Path VARCHAR(MAX)
AS 
	SET NOCOUNT ON 

	SELECT [Id], [Path], [Deleted] 
	FROM   [FS].[WatchedFile] 
	WHERE  [Path] = @Path