CREATE PROC [FS].[Create] 
    @Path varchar(MAX),
    @Deleted nchar(10)
AS 
	SET NOCOUNT ON 
	SET XACT_ABORT ON  
	
	BEGIN TRAN
	
	INSERT INTO [FS].[WatchedFile] ([Path], [Deleted])
	SELECT @Path, @Deleted
	
	-- Begin Return Select <- do not remove
	SELECT [Id], [Path], [Deleted]
	FROM   [FS].[WatchedFile]
	WHERE  [Id] = SCOPE_IDENTITY()
	-- End Return Select <- do not remove
               
	COMMIT