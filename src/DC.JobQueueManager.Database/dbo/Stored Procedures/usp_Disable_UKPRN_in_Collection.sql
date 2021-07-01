CREATE PROCEDURE [dbo].[usp_Disable_UKPRN_in_Collection]
(    @CollectionName VARCHAR(250)
    ,@UKPRN BIGINT
)	   
AS
	BEGIN
	SET NOCOUNT ON;
	DECLARE @OrganisationId INT = (SELECT [OrganisationId] FROM [dbo].[Organisation] WHERE [Ukprn] = @UKPRN);
	DECLARE @CollectionId INT = (SELECT [CollectionId] FROM [dbo].[Collection] WHERE [Name] = @CollectionName);


	IF @CollectionId IS NULL
	BEGIN	
		RAISERROR('Unable to Find Collection : %s',10,1,@CollectionName) WITH NOWAIT;
		RETURN;
	END


	IF @OrganisationId IS NULL
	BEGIN	
		RAISERROR('Unable to Find Organisation with UKPRN : %s',10,1,@UKPRN) WITH NOWAIT;
		RETURN;
	END


	IF NOT EXISTS (	SELECT 1
					FROM [dbo].[OrganisationCollection] OC
					WHERE OC.[OrganisationId] = @OrganisationId
					  AND OC.[CollectionId] = @CollectionId
	)
	 BEGIN
		RAISERROR('Error UKPRN [%I64d] : OrfId [%i] not Found with Collection [%s] : ColId [%i]',17,1,@UKPRN,@OrganisationId,@CollectionName,@CollectionId) WITH NOWAIT;
				--SELECT * FROM [dbo].[Organisation] WHERE [Ukprn] = @UKPRN		

	 END
	--ELSE
	-- BEGIN
	--	IF EXISTS (SELECT 1 FROM [dbo].[OrganisationCollection] OC
	--				WHERE OC.[OrganisationId] = @OrganisationId
	--				  AND OC.[CollectionId] = @CollectionId
	--				  --AND GETUTCDATE() BETWEEN OC.[DateStart]  AND OC.[DateEnd]
	--			 )
	--	BEGIN
	--		UPDATE OC
	--			SET [DateEnd] = GETUTCDATE()
	--		FROM [dbo].[OrganisationCollection] OC
	--				WHERE OC.[OrganisationId] = @OrganisationId
	--				  AND OC.[CollectionId] = @CollectionId
	--				  --AND GETUTCDATE() BETWEEN OC.[DateStart] AND OC.[DateEnd]
	--		RAISERROR('UKPRN : %I64d  - Disable access to Collection : %s',10,1,@UKPRN,@CollectionName) WITH NOWAIT;			
	--	END
	--	ELSE
	--	BEGIN
	--		RAISERROR('Warning | UKPRN : %I64d | Collection : %s - Already Disabled.',10,1,@UKPRN,@CollectionName) WITH NOWAIT;			
	
	--	END		
	-- END	
END	 
GO