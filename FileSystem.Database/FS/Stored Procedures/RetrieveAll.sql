CREATE PROC [FS].[RetrieveAll] 
AS 
	SET NOCOUNT ON 

	SELECT [Id], [Path], [Deleted] 
	FROM   [FS].[WatchedFile]