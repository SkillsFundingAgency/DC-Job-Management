DECLARE @MigrationGuid AS UNIQUEIDENTIFIER = '4B900A74-E2D9-4837-B9A4-9E828752716E';
DECLARE @MigrationDescription AS  NVARCHAR (MAX) = 'Set EmailOnJobCreation for EAS, ESF, DevolvedContracts Collections';
DECLARE @author AS NVARCHAR (MAX) = 'Hari Dupati';

IF NOT EXISTS(SELECT * FROM Migration WHERE MigrationId = @MigrationGuid)
BEGIN TRY
    BEGIN TRANSACTION
	------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Your Code below.
	------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		UPDATE dbo.Collection SET EmailOnJobCreation = 1 WHERE ( CollectionTypeId IN (2,3) OR NAME = 'DevolvedContracts')  AND EmailOnJobCreation <> 1
		
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