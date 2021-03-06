
CREATE PROC [DCFTMigration].[sp_BackupReportsArchive]
AS
BEGIN TRAN

	ALTER TABLE DCFTMigration.ReportsArchive_Backup NOCHECK CONSTRAINT all;

	SET IDENTITY_INSERT DCFTMigration.ReportsArchive_Backup ON
	DELETE FROM  DCFTMigration.ReportsArchive_Backup;

	INSERT INTO DCFTMigration.ReportsArchive_Backup( id,uploadedby,UploadedDateTimeUtc,Year,Period,CollectionTypeId,inSLD,Ukprn,FileName)
	SELECT  id,uploadedby,UploadedDateTimeUtc,Year,Period,CollectionTypeId,inSLD,Ukprn,FileName FROM dbo.ReportsArchive
	SET IDENTITY_INSERT DCFTMigration.ReportsArchive_Backup OFF

	ALTER TABLE DCFTMigration.ReportsArchive_Backup WITH CHECK CHECK CONSTRAINT all;

COMMIT

GO
