
BEGIN

  UPDATE [dbo].[JobSubscriptionTask]
  SET Enabled = 0
  WHERE JobTopicTaskId IN (
	SELECT jst.JobTopicTaskId
		FROM [dbo].[JobSubscriptionTask] jst
		JOIN [dbo].[JobTopicSubscription] jts ON jst.JobTopicId = jts.JobTopicId
		JOIN [dbo].[Collection] c ON c.CollectionId = jts.CollectionId
		WHERE jst.TaskName = 'PublishToBAU'
		AND c.name IN 
		(
		'PE-NCS-Summarisation2021',
		'PE-DC-Summarisation2021',
		'PE-ESF-Summarisation2021',
		'PE-App-Summarisation2021',
		'PE-ALLF-Summarisation',
		'PE-NCS-Summarisation2122',
		'PE-DC-Summarisation2122',
		'PE-ESF-Summarisation2122',
		'PE-App-Summarisation2122'
		)
  )

END
GO
