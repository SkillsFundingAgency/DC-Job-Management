
ALTER ROLE [db_datawriter] ADD MEMBER [JobManagementSchedulerUser];
GO
ALTER ROLE [db_datareader] ADD MEMBER [JobManagementSchedulerUser];
GO
ALTER ROLE [db_datawriter] ADD MEMBER [JobManagementApiUser];
GO
ALTER ROLE [db_datareader] ADD MEMBER [JobManagementApiUser];

GO
ALTER ROLE [DataViewer] ADD MEMBER [JobManagementApiUser];
GO
ALTER ROLE [DataProcessor] ADD MEMBER [JobManagementApiUser];
GO
ALTER ROLE [DataViewer] ADD MEMBER [JobManagementSchedulerUser];
GO
ALTER ROLE [DataViewer] ADD MEMBER [JobManagement_RO_User];
GO
ALTER ROLE [DataProcessor] ADD MEMBER [JobManagementSchedulerUser];
GO
ALTER ROLE [DataViewer] ADD MEMBER [User_DSCI];
GO
