IF NOT EXISTS(SELECT name FROM sys.server_principals WHERE name = 'IIS APPPOOL\ASP.NET v4.0')
BEGIN
CREATE LOGIN [IIS APPPOOL\ASP.NET v4.0]
  FROM WINDOWS WITH DEFAULT_DATABASE=[NietoYosten],
  DEFAULT_LANGUAGE=[us_english]
END
GO
USE NietoYosten
CREATE USER [NietoYostenUser]
  FOR LOGIN [IIS APPPOOL\ASP.NET v4.0]
GO
EXEC sp_addrolemember 'db_owner', 'NietoYostenUser'
GO