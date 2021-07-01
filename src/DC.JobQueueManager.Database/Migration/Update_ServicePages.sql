DECLARE @MigrationGuid AS UNIQUEIDENTIFIER = 'edfb0e16-6b41-4173-9642-c6e6d7ef3e67';
DECLARE @MigrationDescription AS  NVARCHAR (MAX) = 'Initial pupulation of ServiceMessage.ServicePage table';
DECLARE @author AS NVARCHAR (MAX) = 'Jamie Coales';

IF NOT EXISTS(SELECT * FROM Migration WHERE MigrationId = @MigrationGuid)
BEGIN TRY
    BEGIN TRANSACTION
	------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Your Code below.
	------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		INSERT INTO ServiceMessage.ServicePage ([DisplayName], [ControllerName])
		SELECT 'Home', 'Home'
		UNION
		SELECT 'Submission Options', 'SubmissionOptionsAuthorised'
		
	------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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

GO