-- USE [master]

CREATE PROCEDURE deleteExpiredShortUrls
AS
BEGIN
  DELETE FROM master.dbo.ShortUrls WHERE ExpirationAt < GETUTCDATE()
END