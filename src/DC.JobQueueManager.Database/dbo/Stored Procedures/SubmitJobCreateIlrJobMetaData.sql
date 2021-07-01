--	 ,=\.-----""""^==--
--	;;'( ,___, ,/~\;                  
--	'  )/>/   \|-,                  
--	   | `\    | "                  
--	   "   "   "  
CREATE PROCEDURE [dbo].[SubmitJobCreateIlrJobMetaData]
	@jobId int,
	@statusDateTime datetime
AS
BEGIN
	DECLARE @rowCount int
	SET NOCOUNT ON
	BEGIN TRY
		BEGIN TRANSACTION
			SELECT TOP 1 [Id] FROM [dbo].[IlrJobMetaData] WITH (TABLOCKX, HOLDLOCK)

			DECLARE @lastUpdatedDate datetime

			SELECT TOP 1 @lastUpdatedDate = DateTimeSubmittedUtc FROM [dbo].[IlrJobMetaData]
			WHERE JobId = @jobId
			ORDER BY DateTimeSubmittedUtc DESC

			DECLARE @difference int 

			SET @difference = DATEDIFF(second, ISNULL(@lastUpdatedDate, DATEADD(second, -5, @statusDateTime)), @statusDateTime)

			IF @difference >= 5
			BEGIN
				INSERT [dbo].[IlrJobMetaData] (JobId, DateTimeSubmittedUtc)
				VALUES (@jobId, @statusDateTime)

				SET @rowCount = @@ROWCOUNT
			END
		COMMIT
		IF @rowCount > 0
			RETURN 1
		ELSE
			RETURN 0
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()
		IF @@TRANCOUNT > 0
			ROLLBACK
	END CATCH
END