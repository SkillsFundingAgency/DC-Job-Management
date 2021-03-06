
CREATE PROC [DCFTMigration].[sp_MergeReportsArchive]
AS
BEGIN TRAN

SET IDENTITY_INSERT [dbo].[ReportsArchive] ON
	MERGE INTO  dbo.ReportsArchive AS TARGET
	USING  DCFTMigration.ReportsArchive_Staging AS source
		ON source.id = target.id
		
	WHEN not matched then insert(id,uploadedby,UploadedDateTimeUtc,Year,Period,CollectionTypeId,inSLD,Ukprn,FileName)
		VALUES(source.id, source.Uploadedby, source.UploadedDateTimeUtc, source.Year, source.Period,source.CollectionTypeId, source.inSLD, source.Ukprn, source.FileName);

	SET IDENTITY_INSERT [dbo].[ReportsArchive] OFF


COMMIT
GO
