
--Disable Validity Period for RO6 for DAS reports finished collection
UPDATE PeriodEnd.ValidityPeriod 
SET [Enabled] = 0 
WHERE HubPathItemId = 48 
AND [Period] = 6 
AND CollectionYear = 1920

--Disable Validity Period for RO7, R08 for LLV Reoprt
UPDATE PeriodEnd.ValidityPeriod 
SET [Enabled] = 0 WHERE HubPathItemId = 49 
AND [Period] IN (7 , 8) 
AND CollectionYear = 1920

--Disable DAS Reprocessing step before R4 2021.
UPDATE PeriodEnd.ValidityPeriod 
SET [Enabled] = 0
WHERE HubPathItemId = 112 
AND (CollectionYear <  2021 OR (CollectionYear = 2021 AND Period < 4))
