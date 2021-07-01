dotnet.exe ef dbcontext scaffold "Server=(local);Database=JobManagement;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Entities -c JobQueueDataContext --force
rem dotnet.exe ef dbcontext scaffold "Server=(localdb)\MSSQLLOCALDB;Database=FundingClaims;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -o Entities -c FundingClaimsDataContext --force
pause