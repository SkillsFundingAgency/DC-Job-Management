CREATE FUNCTION [dbo].[IsPeriodEndCollectionType]
(
	@collectionType varchar(50)
)
RETURNS bit
AS
BEGIN
	
	declare @result bit;
	Set @result = CASE WHEN @collectionType IN ('PE-ILR', 'PE-NCS','PE-ALLF') THEN 1 ELSE 0 END

	Return @result
END