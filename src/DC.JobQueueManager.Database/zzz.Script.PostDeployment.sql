/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
SET NOCOUNT ON; 

GO
-- Set ExtendedProperties fro DB.
	:r .\z.ExtendedProperties.sql
	
GO

RAISERROR('		   Ref Data',10,1) WITH NOWAIT;
	:r .\ReferenceData\CollectionType.sql
	:r .\zzCollectionsDataScript.sql
	RAISERROR('		   Collections Finished',10,1) WITH NOWAIT;

	:r .\ReferenceData\JobStatusType.sql
	
	:r .\ReferenceData\MCADetail.sql

	:r .\ReferenceData\PeriodEndEmailTemplates.sql
	:r .\ReferenceData\EmailValidityPeriods.sql
	:r .\ReferenceData\SubPathValidityPeriods.sql
	:r .\ReferenceData\ALLFEmailValidityPeriods.sql
	:r .\ReferenceData\ApiAvailability.sql
	:r .\ReferenceData\DisablePublishToBAUTasks.sql
	:r .\ReferenceData\DisableNCSPeriodEndTasks.sql
	:r .\DbRefactor\Add_ESF_ContractReferenceNumber.sql
	:r .\DbRefactor\CopyData_FrmReportsJobMetaData.sql
	:r .\DbRefactor\Remove_FundingClaimsTables.sql
	:r .\DbRefactor\Update_PE_CollectionTypes.sql

	-- Leave the following file always at the end
	:r .\ReferenceData\DeleteUnwanted.sql

	-- Migration Scripts
	:r .\Migration\Update_Collection_EmailOnJobCreation.sql
	:r .\Migration\Update_ServicePages.sql
	:r .\Migration\Migrate_ServicePages.sql
GO
/*
-- Set Default Values on Columns for Start end End
  UPDATE [dbo].[OrganisationCollection]
  SET [DateStart]  = CONVERT(DATETIME,'2018-AUG-01')
     ,[DateEnd]  = CONVERT(DATETIME,'2600-JUL-31')
  WHERE [DateStart] IS NULL
    AND [DateEnd] IS NULL;

RAISERROR('		   Update OrganisationCollection start and end dates : %i Records updated.',10,1,@@ROWCOUNT) WITH NOWAIT;
*/
GO
RAISERROR(' ',10,1) WITH NOWAIT;


RAISERROR('		   Update User Account Passwords',10,1) WITH NOWAIT;
GO

REVOKE REFERENCES ON SCHEMA::[dbo] FROM [DataProcessor];
REVOKE REFERENCES ON SCHEMA::[dbo] FROM [DataViewer];
GO

RAISERROR('		         JobManagementApiUser',10,1) WITH NOWAIT;
ALTER USER [JobManagementApiUser] WITH PASSWORD = N'$(JobManagementApiUserPwd)';
GO

RAISERROR('		         JobManagementSchedulerUser',10,1) WITH NOWAIT;
ALTER USER [JobManagementSchedulerUser] WITH PASSWORD = N'$(JobManagementSchedulerUserPwd)';
GO

RAISERROR('		         JobManagement_RO_User',10,1) WITH NOWAIT;
ALTER USER [JobManagement_RO_User] WITH PASSWORD = N'$(ROUserPassword)';
GO

RAISERROR('		         UserDSCI',10,1) WITH NOWAIT;
ALTER USER [User_DSCI] WITH PASSWORD = N'$(DsciUserPassword)';
GO


:r .\zz.Remove.Old.Objects.sql

GO

RAISERROR('Completed',10,1) WITH NOWAIT;
GO
