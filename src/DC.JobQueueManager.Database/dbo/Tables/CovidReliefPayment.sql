CREATE TABLE [dbo].CovidReliefPayment(
	[UKPRN] bigint NOT NULL PRIMARY KEY,
	[AEB_ILR_Earnings_R09] [money] NULL,
	[AEB_ILR_Earnings_R10] [money] NULL,
	[AEB_ILR_Earnings_R11] [money] NULL,
	[NLAPPS_ILR_Earnings_R09] [money] NULL,
	[NLAPPS_ILR_Earnings_R10] [money] NULL,
	[NLAPPS_ILR_Earnings_R11] [money] NULL,
	[AEB_PRS Payment_R09] [money] NULL,
	[AEB_PRS Payment_R10] [money] NULL,
	[AEB_PRS Payment_R11] [money] NULL,
	[NLAPPS_PRS_Payment_R09] [money] NULL,
	[NLAPPS_PRS_Payment_R10] [money] NULL,
	[NLAPPS_PRS_Payment_R11] [money] NULL
) ON [PRIMARY]
GO