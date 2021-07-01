DECLARE @MigrationGuid AS UNIQUEIDENTIFIER = '9d55f21d-5888-48e2-8479-e1069fd12bb1';
DECLARE @MigrationDescription AS  NVARCHAR (MAX) = 'Copy old service messages to new table and add to submissions page as per story 89175';
DECLARE @author AS NVARCHAR (MAX) = 'Jamie Coales';

IF NOT EXISTS(SELECT * FROM Migration WHERE MigrationId = @MigrationGuid)
BEGIN TRY
    BEGIN TRANSACTION
	------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	-- Your Code below.
	------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		DECLARE @pageId INT = (SELECT [Id]
								FROM [ServiceMessage].[ServicePage]
								WHERE ControllerName = 'SubmissionOptionsAuthorised')
		
		INSERT ServiceMessage.ServiceMessage ([Message], [Enabled], [StartDateTimeUtc], [EndDateTimeUtc], [Headline])
		SELECT 
			[Message], [Enabled], [StartDateTimeUtc], [EndDateTimeUtc], [Headline] 
		FROM [dbo].[ServiceMessage]

		INSERT ServiceMessage.ServicePageMessage
		SELECT @pageId, Id FROM ServiceMessage.ServiceMessage
		
	------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        INSERT INTO Migration (MigrationId, [Description], Author, BUILD_BUILDNUMBER, BUILD_BRANCHNAME, RELEASE_RELEASENAME) 
		SELECT @MigrationGuid, @MigrationDescription, @author, 
				'$(BUILD_BUILDNUMBER)' as BUILD_BUILDNUMBER, '$(BUILD_BRANCHNAME)' AS BUILD_BRANCHNAME, '$(RELEASE_RELEASENAME)' AS RELEASE_RELEASENAME
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