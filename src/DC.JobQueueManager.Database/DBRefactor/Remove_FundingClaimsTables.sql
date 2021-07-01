IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'FundingClaimsReturnPeriodMetaData'))
BEGIN
   DROP TABLE dbo.FundingClaimsReturnPeriodMetaData
END

IF (EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'FundingClaimsCollectionMetaData'))
BEGIN
   DROP TABLE dbo.FundingClaimsCollectionMetaData
END