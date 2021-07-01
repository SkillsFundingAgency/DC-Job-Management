IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'FrmReportsJobMetaData'))
BEGIN
    IF (NOT EXISTS (SELECT top 1 * from [ReportsPublicationJobMetaData]))
    BEGIN
        INSERT INTO [dbo].[ReportsPublicationJobMetaData] ([SourceFolderKey],[SourceContainerName],[JobId],[PeriodNumber],[StorageReference])
        Select [SourceFolderKey],[SourceContainerName],[JobId],[PeriodNumber],[StorageReference] FROM [dbo].FrmReportsJobMetaData
    END
END