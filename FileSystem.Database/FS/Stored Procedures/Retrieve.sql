CREATE PROC [FS].[Retrieve] 
    @Id int
AS 
	SET NOCOUNT ON 

	SELECT [Id], [Path], [Deleted] 
	FROM   [FS].[WatchedFile] 
	WHERE  [Id] = @Id