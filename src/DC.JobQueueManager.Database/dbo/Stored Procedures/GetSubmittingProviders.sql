CREATE PROCEDURE [dbo].[GetSubmittingProviders] 
--DECLARE
	@collectionName VARCHAR(50) -- = 'ILR1819'
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @completeStatus INT = 4
    
	SELECT j.Ukprn
            FROM 
            [dbo].[Job] j 
            INNER JOIN [dbo].[Collection] c ON j.CollectionId = c.CollectionId
            INNER JOIN [dbo].[CollectionType] ct ON c.CollectionTypeId = ct.CollectionTypeId
            WHERE c.[Name] = @collectionName And j.Status =  @completeStatus
            AND CT.Type IN('ILR','ESF','EAS')
            GROUP BY j.Ukprn
END