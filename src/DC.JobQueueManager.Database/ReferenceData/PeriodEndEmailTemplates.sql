
BEGIN
	DECLARE @SummaryOfChanges_Emails TABLE ([HubEmailId] INT, [TemplateId] VARCHAR(250), [TemplateName] VARCHAR(250), [TriggerPointName] VARCHAR(250), [Action] VARCHAR(100));

	MERGE INTO [Mailing].[Email] AS Target
	USING (
			SELECT [HubEmailId], [TemplateId], [TemplateName], [TriggerPointName] 
			FROM 
			(
					SELECT 1 AS [HubEmailId], '3686f36e-2d5f-42f8-ac34-03267c838a43' AS [TemplateId], 'Confirm Collection Closed Email' AS [TemplateName], 'Confirm Collection Closed' AS [TriggerPointName]
				UNION SELECT 2 AS [HubEmailId], '54defa54-1cc7-44ec-a167-84a2bd3757bc' AS [TemplateId], 'Period End Started Email' AS [TemplateName], 'Period End Started' AS [TriggerPointName]
				UNION SELECT 3 AS [HubEmailId], '0d1f5786-8df9-406c-a44e-7708ed52990d' AS [TemplateId], 'Data Warehouse 1 Email' AS [TemplateName], 'Data Warehouse 1' AS [TriggerPointName]
				UNION SELECT 4 AS [HubEmailId], 'b5681027-70f3-4462-ba7e-e7d16f697e0f' AS [TemplateId], 'DAS Start Email' AS [TemplateName], 'DAS Start' AS [TriggerPointName]
				UNION SELECT 5 AS [HubEmailId], '99957613-7aa1-4ae8-bfcd-f1e0cbba57c6' AS [TemplateId], 'FCS Team Handover Part 1 Email' AS [TemplateName], 'FCS Handover Part 1' AS [TriggerPointName]
				UNION SELECT 7 AS [HubEmailId], 'b1f5f992-4c94-4e2f-96b4-d559d848b6a4' AS [TemplateId], 'Standard File Email' AS [TemplateName], 'Standard File Email' AS [TriggerPointName]
				UNION SELECT 9 AS [HubEmailId], '30c636ac-2d35-4ac2-adfb-88fb9b6a6b78' AS [TemplateId], 'Data Warehouse 2 Email' AS [TemplateName], 'Data Warehouse 2' AS [TriggerPointName]
				UNION SELECT 10 AS [HubEmailId], '1eaacf92-326a-46c1-97ea-b71c38c97a05' AS [TemplateId], 'Reports Published Email' AS [TemplateName], 'Reports Published' AS [TriggerPointName]
				UNION SELECT 12 AS [HubEmailId], '6677c42f-1dfd-433b-a76a-4d99e5f1fbe1' AS [TemplateId], 'FCS Broadcast Handover Email' AS [TemplateName], 'FCS Team Handover' AS [TriggerPointName]
				UNION SELECT 13 AS [HubEmailId], '6c1d7aae-a8f0-4804-b921-2a958f8039c6' AS [TemplateId], 'MCA Reports Available Email' AS [TemplateName], 'MCA Reports Published' AS [TriggerPointName]
				UNION SELECT 14 AS [HubEmailId], '27dcd51d-7a77-477d-86e0-98739cd61f1a' AS [TemplateId], 'NCS FCS Handover Email' AS [TemplateName], 'NCS FCS Handover' AS [TriggerPointName]
				UNION SELECT 15 AS [HubEmailId], 'b8953633-83ce-4f41-92a9-2df03f9f3765' AS [TemplateId], 'NCS Confirm Collection Closed Email' AS [TemplateName], 'NCS Confirm Collection Closed' AS [TriggerPointName]
				UNION SELECT 16 AS [HubEmailId], '71640cf6-468a-49a9-94f0-a09ed1dd72d0' AS [TemplateId], 'ALLF FCS Handover Email' AS [TemplateName], 'ALLF FCS Handover' AS [TriggerPointName]
				UNION SELECT 17 AS [HubEmailId], '722ee6d2-67eb-483a-ad75-827721b935aa' AS [TemplateId], 'FCS Team Handover Part 2 Email' AS [TemplateName], 'FCS Handover  Part 2' AS [TriggerPointName]
				
				UNION SELECT 18 AS [HubEmailId], 'df17eb3c-8404-4214-80c0-f4b6495f1e70' AS [TemplateId], 'Data Quality Report Email' AS [TemplateName], 'Data Quality Report' AS [TriggerPointName]
				UNION SELECT 19 AS [HubEmailId], '29c308aa-01cb-44d8-a7ad-de236096ac8b' AS [TemplateId], 'Provider Submissions Report Email' AS [TemplateName], 'Provider Submissions Report' AS [TriggerPointName]
				UNION SELECT 20 AS [HubEmailId], 'df49bca5-8f18-4642-be5f-58a9f806ba16' AS [TemplateId], 'ESFA Non Contracted Devolved Adult Education Activity Report Email' AS [TemplateName], 'ESFA Non Contracted Devolved Adult Education Activity Report' AS [TriggerPointName]
				UNION SELECT 21 AS [HubEmailId], 'bf9a5c36-0282-414d-9cc4-00ec48011c84' AS [TemplateId], 'Data Extract Report Email' AS [TemplateName], 'Data Extract Report' AS [TriggerPointName]
				UNION SELECT 22 AS [HubEmailId], '31089643-55f9-44dc-937e-462ee55662cb' AS [TemplateId], 'Reports Available Email' AS [TemplateName], 'Reports Available Email' AS [TriggerPointName]
			) As NewRecords
		  )
		AS Source([HubEmailId], [TemplateId], [TemplateName], [TriggerPointName])
			ON Target.[HubEmailId] = Source.[HubEmailId]
		WHEN MATCHED 
				AND EXISTS 
					(		SELECT Target.[HubEmailId]
								  ,Target.[TemplateId]
								  ,Target.[TemplateName]
								  ,Target.[TriggerPointName]
						EXCEPT 
							SELECT Source.[HubEmailId]
								  ,Source.[TemplateId]
								  ,Source.[TemplateName]
								  ,Source.[TriggerPointName]
					)
			  THEN UPDATE SET Target.[HubEmailId] = Source.[HubEmailId],
							  Target.[TemplateId] = Source.[TemplateId],
							  Target.[TemplateName] = Source.[TemplateName],
							  Target.[TriggerPointName] = Source.[TriggerPointName]

		WHEN NOT MATCHED BY TARGET THEN INSERT([HubEmailId], [TemplateId], [TemplateName], [TriggerPointName]) 
									   VALUES ([HubEmailId], [TemplateId], [TemplateName], [TriggerPointName])
		WHEN NOT MATCHED BY SOURCE THEN DELETE
		OUTPUT Inserted.[HubEmailId], Inserted.[TemplateId], Inserted.[TemplateName], Inserted.[TriggerPointName], $action INTO @SummaryOfChanges_Emails([HubEmailId], [TemplateId], [TemplateName], [TriggerPointName], [Action])
	;

		DECLARE @AddCount_CT INT, @UpdateCount_CT INT, @DeleteCount_CT INT
		SET @AddCount_CT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Emails WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Emails WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_Emails WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		        %s - Added %i - Update %i - Delete %i',10,1,'     Email', @AddCount_CT, @UpdateCount_CT, @DeleteCount_CT) WITH NOWAIT;
END
GO
