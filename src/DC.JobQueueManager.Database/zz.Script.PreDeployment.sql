/*
Pre-Deployment Script Template							
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

	:r .\Migration\Remove_LarsJobSubscriptionTask.sql
    :r .\Migration\Remove_Recipient_Duplicates.sql

--DECLARE @pk_Name varchar(255), @sql nvarchar(MAX)
---- TEMPORARY CODE TO ALLOW FOR A CHANGE TO PRIMARY KEY AND ADDITIONAL OF NEW COMPOUND KEY - WORKS BEST WHEN TABLE IS EMPTIED. THIS SHOULD BE REMOVED FOLLOWING A SUCCESSFUL PROD DEPLOYMENT.
--IF EXISTS (SELECT 1
--               FROM   INFORMATION_SCHEMA.COLUMNS
--               WHERE  TABLE_NAME = 'ValidityPeriod'
--                      AND COLUMN_NAME = 'Id'
--                      AND TABLE_SCHEMA='PeriodEnd')
--  BEGIN
--	DELETE FROM [PeriodEnd].ValidityPeriod
--  END


---- TEMPORARY CODE TO DROP COLLECTIONID from ValidityPeriod table.  This can be removed after sucessful deployment
IF EXISTS (SELECT 1
               FROM   INFORMATION_SCHEMA.COLUMNS
               WHERE  TABLE_NAME = 'ValidityPeriod'
                      AND COLUMN_NAME = 'CollectionId'
                      AND TABLE_SCHEMA='PeriodEnd')
  BEGIN
		DELETE FROM [PeriodEnd].[ValidityPeriod]
  END

GO

RAISERROR('		   Pre Deployment Completed',10,1) WITH NOWAIT;

GO
