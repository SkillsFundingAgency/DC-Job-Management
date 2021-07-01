DECLARE @MigrationGuid AS UNIQUEIDENTIFIER = '39B6AFDE-E00F-4D69-9B03-28ECA498DD74';
DECLARE @MigrationDescription AS  NVARCHAR (MAX) = 'Remove duplicate email addresses from Mailing.Recipient and clean up corresponding RecipientGroupRecipient table, needed to fix bug as per story 109817';
DECLARE @author AS NVARCHAR (MAX) = 'David Gill';

IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'dbo' 
            AND  TABLE_NAME = 'Migration'))
BEGIN
	IF NOT EXISTS(SELECT * FROM Migration WHERE MigrationId = @MigrationGuid)
	BEGIN TRY
		BEGIN TRANSACTION
		------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		-- Your Code below.
		------------------------------------------------------------------------------------------------------------------------------------------------------------------------
			update [Mailing].[RecipientGroupRecipient] set RecipientId = j1.MinRecipientId
			from [Mailing].[RecipientGroupRecipient] t1
			inner join (
			select rgr.RecipientGroupId, g.RecipientId, g.EmailAddress, g.MinRecipientId from 
			[Mailing].[RecipientGroupRecipient] rgr
				inner join (
				Select a.*, r.RecipientId
				From [Mailing].[Recipient] r
				inner join (
					SELECT r.[EmailAddress], Min(RecipientId) MinRecipientId
					  FROM [Mailing].[Recipient] r
					  Group by r.EmailAddress
					  having count(*) > 1) a on a.EmailAddress = r.EmailAddress
					  where r.RecipientId <> a.MinRecipientId) g on g.RecipientId = rgr.RecipientId) j1 on t1.RecipientGroupId = j1.RecipientGroupId and t1.RecipientId = j1.RecipientId

			delete r from [Mailing].[Recipient] r
			join (
				Select a.*, r.RecipientId
				From [Mailing].[Recipient] r
				inner join (
					SELECT r.[EmailAddress], Min(RecipientId) MinRecipientId
					FROM [Mailing].[Recipient] r
					Group by r.EmailAddress
					having count(*) > 1) a on a.EmailAddress = r.EmailAddress
					where r.RecipientId <> a.MinRecipientId) g on r.RecipientId = g.RecipientId
		
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
END
GO