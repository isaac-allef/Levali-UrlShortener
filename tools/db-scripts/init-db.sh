/opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P SqlServer2022! -d master -i ./migration.sql
/opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P SqlServer2022! -d master -i ./procedures.sql
/opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P SqlServer2022! -d msdb -i ./jobs.sql
