﻿CREATE TABLE [dbo].[Covid19ReliefNLAppsMonthlyCap](
	[UKPRN] [bigint] NOT NULL,
	[ProviderName] [nvarchar](255) NULL,
	[EligibleApps] [nvarchar](255) NULL,
	[1920MCV] [nvarchar](255) NULL,
	[2021MCV] [nvarchar](255) NULL,
	[earningsjuly19] [nvarchar](255) NULL,
	[earningsaug19] [nvarchar](255) NULL,
	[earningssept19] [nvarchar](255) NULL,
	[earningsoct19] [nvarchar](255) NULL,
	[julyearning1920mcv] [nvarchar](255) NULL,
	[augearning1920mcv] [nvarchar](255) NULL,
	[septearning1920mcv] [nvarchar](255) NULL,
	[octearning1920mcv] [nvarchar](255) NULL,
	[monthlycapjuly] [nvarchar](255) NULL,
	[monthlycapaug] [nvarchar](255) NULL,
	[monthlycapsept] [nvarchar](255) NULL,
	[monthlycapoct] [nvarchar](255) NULL,
	[July1920v2021check] [nvarchar](255) NULL,
	[Aug1920v2021check] [nvarchar](255) NULL,
	[Sept1920v2021check] [nvarchar](255) NULL,
	[Oct1920v2021check] [nvarchar](255) NULL,
	[Julyok] [nvarchar](255) NULL,
	[Augok] [nvarchar](255) NULL,
	[Septok] [nvarchar](255) NULL,
	[Octok] [nvarchar](255) NULL,
	CONSTRAINT [PK_Covid19ReliefNLAppsMonthlyCap] PRIMARY KEY CLUSTERED ([UKPRN] ASC)
)
