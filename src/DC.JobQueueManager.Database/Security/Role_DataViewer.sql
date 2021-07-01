
CREATE ROLE [DataViewer] AUTHORIZATION [dbo]
GO

-- Grant access rights to a specific schema in the database
GRANT
	SELECT, 
	VIEW DEFINITION 
ON SCHEMA::[dbo]
	TO [DataViewer]
GO

GRANT 
	SELECT, 
	VIEW DEFINITION 
ON SCHEMA::[PeriodEnd]
	TO [DataViewer]
GO

GRANT
	SELECT, 
	VIEW DEFINITION 
ON SCHEMA::[Mailing]
	TO [DataViewer]
GO

GRANT
	SELECT, 
	VIEW DEFINITION 
ON SCHEMA::[ServiceMessage]
	TO [DataViewer]
GO