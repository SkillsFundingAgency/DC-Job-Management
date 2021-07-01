
SET NOCOUNT ON;
DECLARE @CollectionNameToAssignTo VARCHAR(250) ='XXXXXXXXX'  -- SELECT [Name] FROM [dbo].[Collection]
	,@StartDateTime DATETIME2 = NULL   -- Will be set to NOW if Left as NULL
	,@EndDateTime DATETIME2 = NULL	   -- Will be set to 2600-JUL-31 if Left as NULL

DECLARE @Providers TABLE (  UKPRN BIGINT,
							OrgName VARCHAR(250),
							CollectionName VARCHAR(250)
							PRIMARY KEY (UKPRN, CollectionName)
						 )

INSERT INTO @Providers( [UKPRN],[OrgName],[CollectionName])
SELECT DISTINCT [UKPRN],[OrgName],@CollectionNameToAssignTo AS [CollectionName]
FROM
(

---- ="UNION SELECT " & C1 & " AS UKPRN, '" & A1 & "'AS OrgName"

-------------------------------------------------------- PUT DATA - START--------------------------------------------------------    
SELECT -9999, 'XXX'
-------------------------------------------------------- PUT DATA - START-------------------------------------------------------- 
) as NewRecords

--SELECT COUNT(*) FROM @Providers

SET @StartDateTime = ISNULL(@StartDateTime,GETUTCDATE());
SET @EndDateTime = ISNULL(@EndDateTime,CONVERT(DATETIME,'2600-JUL-31'));

DECLARE @UKPRN BIGINT ,@OrgName VARCHAR(250),@OrgEmail VARCHAR(250), @CollectionName VARCHAR(250)

DECLARE Providers_Cursor CURSOR READ_ONLY FOR 
SELECT 
	[UKPRN],[OrgName],[CollectionName] 
FROM @Providers 

OPEN Providers_Cursor;  
FETCH NEXT FROM Providers_Cursor INTO @UKPRN, @OrgName,@CollectionName;  
WHILE @@FETCH_STATUS = 0  
   BEGIN  
	--SELECT  @UKPRN as UKPRN ,@OrgName as OrgName, @CollectionName as CollectionName, @StartDateTime as StartDateTime, @EndDateTime as EndDateTime
	
	EXEC [dbo].[usp_Add_UKPRN_to_Collection]
			@CollectionName = @CollectionName
		   ,@UKPRN = @UKPRN
		   ,@OrgName = @OrgName
		   ,@StartDateTime = @StartDateTime
		   ,@EndDateTime = @EndDateTime

      FETCH NEXT FROM Providers_Cursor INTO @UKPRN,@OrgName,@CollectionName; 
   END;  
CLOSE Providers_Cursor;  
DEALLOCATE Providers_Cursor;  
GO  

--SELECT * FROM [dbo].[vw_CurrentCollectionReturnPeriods]
GO
