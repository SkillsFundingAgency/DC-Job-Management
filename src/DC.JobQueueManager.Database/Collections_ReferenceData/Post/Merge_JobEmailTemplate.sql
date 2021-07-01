
DECLARE @SummaryOfChanges_JobEmailTemplate TABLE ([EventId] varchar(500), [Action] VARCHAR(100));

MERGE INTO [dbo].[JobEmailTemplate] AS Target
USING (
		SELECT [TemplateOpenPeriod],[TemplateClosePeriod], [JobStatus], [Active],[CollectionId] FROM @TempJobEmailTemplate
	  )
	AS Source([TemplateOpenPeriod],[TemplateClosePeriod], [JobStatus], [Active],[CollectionId])
	ON Target.[TemplateOpenPeriod] = Source.[TemplateOpenPeriod]
		And Target.[CollectionId] = Source.[CollectionId]
	WHEN MATCHED 
			AND EXISTS 
				(		SELECT Target.[JobStatus]
							  ,Target.[Active]
							  ,Target.[CollectionId]
							  ,Target.[TemplateOpenPeriod]
							  ,Target.[TemplateClosePeriod]
					EXCEPT 
						SELECT source.[JobStatus]
							  ,source.[Active]
							  ,source.[CollectionId]
							  ,source.[TemplateOpenPeriod]
							  ,source.[TemplateClosePeriod]
				)
		  THEN UPDATE SET Target.[JobStatus] = Source.[JobStatus],
			              Target.[Active] = Source.[Active],
						  Target.[CollectionId] = Source.[CollectionId],
						  Target.[TemplateOpenPeriod] = source.[TemplateOpenPeriod],
						  Target.[TemplateClosePeriod] = source.[TemplateClosePeriod]
	WHEN NOT MATCHED BY TARGET THEN INSERT([TemplateOpenPeriod],[TemplateClosePeriod], [JobStatus], [Active],[CollectionId]) 
								   VALUES ([TemplateOpenPeriod],[TemplateClosePeriod], [JobStatus], [Active],[CollectionId])
	WHEN NOT MATCHED BY SOURCE THEN DELETE
	OUTPUT Inserted.[TemplateOpenPeriod],$action INTO @SummaryOfChanges_JobEmailTemplate([EventId],[Action])
;

	DECLARE @AddCount_JET INT, @UpdateCount_JET INT, @DeleteCount_JET INT
	SET @AddCount_JET  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobEmailTemplate WHERE [Action] = 'Insert' GROUP BY Action),0);
	SET @UpdateCount_JET = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobEmailTemplate WHERE [Action] = 'Update' GROUP BY Action),0);
	SET @DeleteCount_JET = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_JobEmailTemplate WHERE [Action] = 'Delete' GROUP BY Action),0);

	RAISERROR('		      %s - Added %i - Update %i - Delete %i',10,1,'     JobEmailTemplate', @AddCount_JET, @UpdateCount_JET, @DeleteCount_JET) WITH NOWAIT;

