-- USE msdb;  
  
EXEC dbo.sp_add_job  
    @job_name = N'Delete expired short urls';   --Job Name

EXEC sp_add_jobstep  
    @job_name = N'Delete expired short urls',  
    @step_name = N'Run Procedure',              --Step Name
    @subsystem = N'TSQL',                       --Step Type 
    @command = N'EXEC deleteExpiredShortUrls'   --Command to be executed
  
EXEC dbo.sp_add_schedule  
    @schedule_name = N'RunDelete',              --Schedule Name
    @freq_type = 4,                             --Only one day execution  
	@freq_interval = 1,
    @freq_subday_type = 0x4,                    --Execution interval in minutes  
	@freq_subday_interval = 2;                  --Execution interval 
  
EXEC sp_attach_schedule  
   @job_name = N'Delete expired short urls',  
   @schedule_name = N'RunDelete';  
  
EXEC dbo.sp_add_jobserver  
    @job_name = N'Delete expired short urls';  