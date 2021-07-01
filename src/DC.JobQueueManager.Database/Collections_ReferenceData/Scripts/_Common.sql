
-- Common Job Message keys
INSERT INTO @TempJobMessageKey

SELECT NULL AS CollectionId, N'ReturnPeriod' AS MessageKey,  NULL AS IsFirstStage 
UNION SELECT NULL AS CollectionId, N'CollectionName' AS MessageKey,  NULL AS IsFirstStage 
UNION SELECT NULL AS CollectionId, N'ReturnPeriodName' AS MessageKey,  NULL AS IsFirstStage 
UNION SELECT NULL AS CollectionId, N'CollectionYear' AS MessageKey,  NULL AS IsFirstStage 