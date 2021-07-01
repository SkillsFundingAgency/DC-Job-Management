CREATE PROCEDURE [dbo].[BulkAddProviders] 
	-- Add the parameters for the stored procedure here
	@ukprns varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT Organisation(Ukprn)  
	SELECT Ukprns.ukprn from OPENJSON(@ukprns) with (ukprn bigint 'strict $') Ukprns
	WHERE Ukprns.ukprn NOT IN (SELECT Ukprn FROM Organisation);  
END;