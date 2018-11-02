CREATE PROC [FS].[Delete] 
    @Id int
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN

	UPDATE [FS].[WatchedFile]
	SET [Deleted] = 1
	WHERE  [Id] = @Id

	COMMIT