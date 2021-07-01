
RAISERROR(' Remove Old Non required Objects',10,1) WITH NOWAIT;

DROP PROCEDURE IF EXISTS [dbo].[usp_Add_OrganisationToCollections];
GO
DROP TABLE IF EXISTS [DataLoad].[ILR1819];
GO
DROP TABLE IF EXISTS [DataLoad].[ILR1920];
GO
DROP TABLE IF EXISTS [DataLoad].[EAS];
GO
DROP TABLE IF EXISTS [dbo].[JobTopicTask]
GO
DROP TABLE IF EXISTS [dbo].[JobTopic]
GO

-- Remove Objects from Dataload Schema as no longer needed
GO
DROP PROCEDURE IF EXISTS [DataLoad].[usp_Process_ILR1819_Standard];
GO
DROP PROCEDURE IF EXISTS [DataLoad].[usp_Process_ILR1819_Periodic];
GO
DROP PROCEDURE IF EXISTS [DataLoad].[usp_Process_FundingClaim];
GO
DROP PROCEDURE IF EXISTS [DataLoad].[usp_Process_ESF_R1];
GO
DROP PROCEDURE IF EXISTS [DataLoad].[usp_Process_ESF_R2];
GO
DROP PROCEDURE IF EXISTS [DataLoad].[usp_Process_EAS1819];
GO
DROP PROCEDURE IF EXISTS [DataLoad].[usp_Process__Providers];
GO

DROP TABLE IF EXISTS [DataLoad].[ESF]
GO
DROP TABLE IF EXISTS [DataLoad].[EFS]
GO
DROP TABLE IF EXISTS [DataLoad].[EAS1819]
GO
DROP TABLE IF EXISTS [DataLoad].[ESF_R1]
GO
DROP TABLE IF EXISTS [DataLoad].[ESF-R2]
GO
DROP TABLE IF EXISTS [DataLoad].[FundingClaim]
GO
DROP TABLE IF EXISTS [DataLoad].[ILR1819_Periodic]
GO
DROP TABLE IF EXISTS [DataLoad].[ILR1819_Standard]
GO
DROP TABLE IF EXISTS [DataLoad].[ILR1920_Periodic]
GO
DROP TABLE IF EXISTS [DataLoad].[ILR1920_Standard]
GO

IF  (
		(EXISTS (SELECT * FROM sys.schemas s WHERE s.name = 'DataLoad'))
	AND ((SELECT COUNT(*) FROM Sys.tables t INNER JOIN sys.schemas s ON s.schema_id = t.schema_id WHERE s.name = 'DataLoad')=0)
)
BEGIN
    RAISERROR('		   Drop Schema [DataLoad] as no Object found in schema',10,1) WITH NOWAIT;    
	DROP SCHEMA IF EXISTS [DataLoad];
END
GO
