SET NOCOUNT ON

DECLARE @SummaryOfChanges_EmailValidityPeriods TABLE ([hubEmailId] int,[email] VARCHAR(100), PeriodNumbers VARCHAR(50), [Action] VARCHAR(100));

DECLARE @listOfPeriods table([period] tinyint)
insert @listOfPeriods([period]) values(55),(56),(57),(58),(59),(60),(61),(62),(63),(64),(65),(66),(67),(68),(69),(70),(71),(72),(73),(74),(75),(76),(77),(78),(79),(80),(81),(82),(83),(84)

DECLARE @listOfEmails table([hubEmailId] int, [email] nvarchar(200))
insert @listOfEmails([hubEmailId], [email]) VALUES
	(16, 'ALLF FCS Handover Email')

DECLARE @hubEmailId int
DECLARE @email nvarchar(200)
DECLARE @period tinyint

DECLARE db_cursor CURSOR FOR
SELECT [hubEmailId], [email]
FROM @listOfEmails

OPEN db_cursor
FETCH NEXT FROM db_cursor into @hubEmailId, @email

WHILE @@FETCH_STATUS = 0
BEGIN
	DECLARE period_cursor CURSOR FOR
	SELECT [period] FROM @listOfPeriods
	OPEN period_cursor
	FETCH NEXT FROM period_cursor into @period
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF NOT EXISTS(SELECT [HubEmailId],[Period] FROM [PeriodEnd].[EmailValidityPeriod] WHERE [HubEmailId] = @hubEmailId AND [Period] = @period)
		BEGIN
			INSERT [PeriodEnd].[EmailValidityPeriod]([HubEmailId], [Period]) VALUES(@hubEmailId, @period)
			INSERT @SummaryOfChanges_EmailValidityPeriods VALUES(@hubEmailId, @email, @period, 'Insert')
		END
		FETCH NEXT FROM period_cursor INTO @period
	END
	CLOSE period_cursor
	DEALLOCATE period_cursor

	FETCH NEXT FROM db_cursor INTO @hubEmailId, @email
END
CLOSE db_cursor
DEALLOCATE db_cursor

DECLARE @AddCount_CT INT, @DeleteCount_CT INT = 0

SET @AddCount_CT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_EmailValidityPeriods WHERE [Action] = 'Insert' GROUP BY Action),0);

RAISERROR('		        %s - Added %i - Delete %i',10,1,'     EmailValidityPeriod', @AddCount_CT, @DeleteCount_CT) WITH NOWAIT;

GO