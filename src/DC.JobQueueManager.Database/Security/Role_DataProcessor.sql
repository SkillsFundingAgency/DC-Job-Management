
CREATE ROLE [DataProcessor] AUTHORIZATION [dbo]
GO

-- Grant access rights to a specific schema in the database
GRANT 
	DELETE, 
	EXECUTE, 
	INSERT, 
	SELECT, 
	UPDATE, 
	VIEW DEFINITION 
ON SCHEMA::[dbo]
	TO [DataProcessor]
GO

-- Grant access rights to a specific schema in the database

GRANT 
	DELETE, 
	EXECUTE, 
	INSERT, 
	SELECT, 
	UPDATE, 
	VIEW DEFINITION 
ON SCHEMA::[PeriodEnd]
	TO [DataProcessor]
GO

GRANT 
	DELETE, 
	EXECUTE, 
	INSERT, 
	SELECT, 
	UPDATE, 
	VIEW DEFINITION 
ON SCHEMA::[Mailing]
	TO [DataProcessor]
GO

GRANT 
	DELETE, 
	EXECUTE, 
	INSERT, 
	SELECT, 
	UPDATE, 
	VIEW DEFINITION 
ON SCHEMA::[ServiceMessage]
	TO [DataProcessor]
GO
GO

GRANT 
    ALTER,
	DELETE, 
	EXECUTE, 
	INSERT, 
	SELECT, 
	UPDATE, 
	VIEW DEFINITION 
ON SCHEMA::[DCFTMigration]
	TO [DataProcessor]
GO