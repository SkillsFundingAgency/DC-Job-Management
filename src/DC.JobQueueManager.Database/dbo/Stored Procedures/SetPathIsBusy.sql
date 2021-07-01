CREATE PROCEDURE [dbo].[SetPathIsBusy]
	@pathId int,
	@isBusy bit
AS
BEGIN
	DECLARE @rowCount int
	SET NOCOUNT ON
	BEGIN TRY
		BEGIN TRANSACTION
			BEGIN
				UPDATE [PeriodEnd].[Path]
				SET IsBusy = @isBusy
				WHERE HubPathId = @pathId 
				AND IsBusy != @IsBusy
				SET @rowCount = @@ROWCOUNT
			END
		COMMIT
		IF @rowCount > 0
			SELECT 1
		ELSE
			SELECT 0
	END TRY
	BEGIN CATCH
		SELECT ERROR_MESSAGE()
		IF @@TRANCOUNT > 0
			ROLLBACK
	END CATCH
END