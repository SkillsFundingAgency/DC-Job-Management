DECLARE @MigrationGuid AS UNIQUEIDENTIFIER = '31ef6714-f160-4cd1-9764-db5b3e53a846';
DECLARE @MigrationDescription AS  NVARCHAR (MAX) = 'Removal of incorrect LARS tasks';
DECLARE @author AS NVARCHAR (MAX) = 'Martin Freshwater';

IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'Migration'))
BEGIN
	IF NOT EXISTS(SELECT * FROM Migration WHERE MigrationId = @MigrationGuid)
	BEGIN TRY
		BEGIN TRANSACTION
	
			DELETE FROM [dbo].[JobSubscriptionTask]
			WHERE TaskName IN ('TaskLarsPublishStorage', 'TaskLarsPublishSearch', 'TaskLarsPublishDB')

			INSERT INTO Migration (MigrationId, [Description], Author, BUILD_BUILDNUMBER, BUILD_BRANCHNAME, RELEASE_RELEASENAME) 
			SELECT @MigrationGuid, @MigrationDescription, @author, 
					'$(BUILD_BUILDNUMBER)' as BUILD_BUILDNUMBER, '$(BUILD_BRANCHNAME)' as BUILD_BRANCHNAME, '$(RELEASE_RELEASENAME)' as RELEASE_RELEASENAME
			RAISERROR ('Executed Migration script: %s', 10,1,@MigrationDescription) WITH NOWAIT
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		RAISERROR ('Failed Executing Migration script: %s', 10,1,@MigrationDescription) WITH NOWAIT
		IF @@TRANCOUNT > 0
			ROLLBACK TRAN
		DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE()
		DECLARE @ErrorSeverity INT = ERROR_SEVERITY()
		DECLARE @ErrorState INT = ERROR_STATE()
		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
	ELSE
		RAISERROR ('%s - already executed.', 10,1,@MigrationDescription) WITH NOWAIT;

END
GO