CREATE TABLE [dbo].[ApiAvailability](
	[ApiName] NVARCHAR(200) NOT NULL,
	[Process] NVARCHAR(200) NOT NULL,
	[Enabled] BIT NOT NULL default(1),
 CONSTRAINT [PK_ApiAvailability] PRIMARY KEY ([ApiName] ASC, [Process] ASC)
)