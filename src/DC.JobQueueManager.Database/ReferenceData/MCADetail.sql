
BEGIN

	DECLARE @SummaryOfChanges_MCADetail TABLE ([Ukprn] bigint, GLACode nvarchar(50), SOFCode int , [Action] VARCHAR(100));

	MERGE INTO [dbo].[MCADetail] AS Target
	USING (VALUES
			(90000439, N'CPCA',   115, '2019-08-01', null, 1920, null),
			(90000440, N'London', 116, '2019-08-01', null, 1920, null),
			(90000441, N'GMCA',   110, '2019-08-01', null, 1920, null),
			(90000442, N'LCRCA',  111, '2019-08-01', null, 1920, null),
			(90000443, N'TVCA',   114, '2019-08-01', null, 1920, null),
			(90000444, N'WECA',   113, '2019-08-01', null, 1920, null),
			(90000445, N'WMCA',   112, '2019-08-01', null, 1920, null),
			(90000449, N'NTCA',   117, '2020-08-01', null, 2021, null),
			(90001039, N'WYCA',   119, '2021-08-01', null, 2122, null),
			(90001041, N'SCRCA',  118, '2021-08-01', null, 2122, null)
		  )
		AS Source([Ukprn], GLACode, SOFCode, EffectiveFrom, EffectiveTo, AcademicYearFrom, AcademicYearTo)
			ON Target.Ukprn = Source.Ukprn
		WHEN MATCHED 
				AND EXISTS 
					(		SELECT Target.[Ukprn]
								  ,Target.GLACode
								  ,Target.SOFCode
								  ,Target.EffectiveFrom
								  ,Target.EffectiveTo
								  ,Target.AcademicYearFrom
								  ,Target.AcademicYearTo
						EXCEPT 
							SELECT Source.[Ukprn]
								  ,Source.GLACode
								  ,Source.SOFCode
								  ,Source.EffectiveFrom
								  ,Source.EffectiveTo
								  ,Source.AcademicYearFrom
								  ,Source.AcademicYearTo
					)
			  THEN UPDATE SET Target.[Ukprn] = Source.[Ukprn],
							  Target.GLACode = Source.GLACode,
							  Target.SOFCode = Source.SOFCode,
							  Target.EffectiveFrom = Source.EffectiveFrom,
							  Target.EffectiveTo = Source.EffectiveTo,
							  Target.AcademicYearFrom = Source.AcademicYearFrom,
							  Target.AcademicYearTo = Source.AcademicYearTo

		WHEN NOT MATCHED BY TARGET THEN INSERT([Ukprn], GLACode, SOFCode, EffectiveFrom, EffectiveTo, AcademicYearFrom, AcademicYearTo) 
									   VALUES ([Ukprn], GLACode, SOFCode, EffectiveFrom, EffectiveTo, AcademicYearFrom, AcademicYearTo)
		WHEN NOT MATCHED BY SOURCE THEN DELETE
		OUTPUT Inserted.Ukprn,$action INTO @SummaryOfChanges_MCADetail(Ukprn,[Action])
	;

		DECLARE @AddCount_MCADetail_CT INT, @UpdateCount_MCADetail_CT INT, @DeleteCount_MCADetail_CT INT
		SET @AddCount_MCADetail_CT  = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_MCADetail WHERE [Action] = 'Insert' GROUP BY Action),0);
		SET @UpdateCount_MCADetail_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_MCADetail WHERE [Action] = 'Update' GROUP BY Action),0);
		SET @DeleteCount_MCADetail_CT = ISNULL((SELECT Count(*) FROM @SummaryOfChanges_MCADetail WHERE [Action] = 'Delete' GROUP BY Action),0);

		RAISERROR('		        %s - Added %i - Update %i - Delete %i',10,1,'     MCADetail', @AddCount_MCADetail_CT, @UpdateCount_MCADetail_CT, @DeleteCount_MCADetail_CT) WITH NOWAIT;
END
GO
