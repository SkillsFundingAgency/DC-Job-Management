CREATE PROCEDURE PeriodEnd.PeriodEndClearDown
	@period INT,
	@collectionName VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

    BEGIN TRAN cleardown

		DELETE pij
		FROM PeriodEnd.PathItemJob pij
		INNER JOIN PeriodEnd.PathItem pi ON pij.PathItemId = pi.PathItemId
		INNER JOIN PeriodEnd.Path p ON pi.PathId = p.PathId
		INNER JOIN PeriodEnd.PeriodEnd pe ON p.PeriodEndId = pe.PeriodEndId
		INNER JOIN dbo.ReturnPeriod rp ON pe.PeriodId = rp.ReturnPeriodId
		INNER JOIN dbo.Collection c ON c.CollectionId = rp.CollectionId
		WHERE rp.PeriodNumber = @period AND c.Name = @collectionName

		DELETE pi 
		FROM PeriodEnd.PathItem pi
		INNER JOIN PeriodEnd.Path p ON pi.PathId = p.PathId
		INNER JOIN PeriodEnd.PeriodEnd pe ON p.PeriodEndId = pe.PeriodEndId
		INNER JOIN dbo.ReturnPeriod rp ON pe.PeriodId = rp.ReturnPeriodId
		INNER JOIN dbo.Collection c ON c.CollectionId = rp.CollectionId
		WHERE rp.PeriodNumber = @period AND c.Name = @collectionName

		DELETE p
		FROM PeriodEnd.Path p
		INNER JOIN PeriodEnd.PeriodEnd pe ON p.PeriodEndId = pe.PeriodEndId
		INNER JOIN dbo.ReturnPeriod rp ON pe.PeriodId = rp.ReturnPeriodId
		INNER JOIN dbo.Collection c ON c.CollectionId = rp.CollectionId
		WHERE rp.PeriodNumber = @period AND c.Name = @collectionName

		DELETE pe
		FROM PeriodEnd.PeriodEnd pe
		INNER JOIN dbo.ReturnPeriod rp ON pe.PeriodId = rp.ReturnPeriodId
		INNER JOIN dbo.Collection c ON c.CollectionId = rp.CollectionId
		WHERE rp.PeriodNumber = @period AND c.Name = @collectionName

	COMMIT TRAN cleardown
END
GO
