--If we have a migration to do then use the path to update collection types accordingly
If EXISTS(SELECT TOP 1 * FROM dbo.CollectionType WHERE CollectionTypeId = 7)
BEGIN
	
	--Update PE collections
	UPDATE dbo.Collection SET CollectionTypeId = CASE WHEN CollectionId IN (150,151) THEN 14 -- NCS
											WHEN CollectionId = 121 THEN 15 -- ALLF
											ELSE 14 END -- ILR
	WHERE CollectionTypeId = 7

	--UPDATE reports Archive
	UPDATE dbo.ReportsArchive SET CollectionTypeId = 14 WHERE CollectionTypeId = 7


	-- Remove the original collection type
	DELETE FROM dbo.CollectionType WHERE CollectionTypeId = 7
END