
BEGIN

	UPDATE [dbo].[JobSubscriptionTask]
	SET Enabled = 0
	WHERE JobTopicTaskId IN (
		SELECT jst.JobTopicTaskId
			FROM [dbo].[JobSubscriptionTask] jst
			JOIN [dbo].[JobTopicSubscription] jts ON jst.JobTopicId = jts.JobTopicId
			JOIN [dbo].[Collection] c ON c.CollectionId = jts.CollectionId
			WHERE c.Name IN 
			(
			'PE-NCS-Summarisation2021',
			'PE-NCS-DataExtract-Report2021',
			'PE-NCS-Summarisation2122',
			'PE-NCS-DataExtract-Report2122'
			)
	);

	UPDATE [dbo].[Collection] 
	SET ProcessingOverrideFlag = 0, IsOpen = 0
	WHERE Name IN 
	(
	'PE-NCS-Summarisation2021',
	'PE-NCS-DataExtract-Report2021',
	'PE-NCS-Summarisation2122',
	'PE-NCS-DataExtract-Report2122'
	)

END
GO
