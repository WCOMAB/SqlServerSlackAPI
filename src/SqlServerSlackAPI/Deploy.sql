/*
 *
 * Enable CLR in master database
 *
 */
USE master
GO
sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO
sp_configure 'clr enabled', 1;
GO
RECONFIGURE;
GO

/*
 *
 * Enable CLR in target database
 *
 */
USE SlackTestDb
GO
sp_configure 'show advanced options', 1;
GO
RECONFIGURE;
GO
sp_configure 'clr enabled', 1;
GO

/*
 *
 * Remove any previously installed functions/procedures
 *
 */
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SlackChatPostMessage]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
    DROP FUNCTION [dbo].SlackChatPostMessage
GO

/*
 *
 * Remove any previously installed assemblies keys from master
 *
 */
USE master
IF EXISTS(SELECT name FROM sys.server_principals WHERE name = 'SqlServerSlackAPILogin')
    DROP LOGIN SqlServerSlackAPILogin
GO

IF EXISTS(SELECT name FROM sys.asymmetric_keys WHERE name = 'SqlServerSlackAPIKey')
    DROP ASYMMETRIC KEY SqlServerSlackAPIKey
GO

/*
 *
 * Install new assembly keys in master
 *
 */
CREATE ASYMMETRIC KEY SqlServerSlackAPIKey FROM EXECUTABLE FILE = 'C:\temp\dev\github\SqlServerSlackAPI\src\SqlServerSlackAPI\bin\debug\SqlServerSlackAPI.dll'
GO
CREATE LOGIN SqlServerSlackAPILogin FROM ASYMMETRIC KEY SqlServerSlackAPIKey
GO
GRANT EXTERNAL ACCESS ASSEMBLY TO SqlServerSlackAPILogin
GO

/*
 *
 * Remove any previously installed assembly from target database
 *
 */
USE SlackTestDb
IF  EXISTS (SELECT * FROM sys.assemblies asms WHERE asms.name = N'SqlServerSlackAPI' and is_user_defined = 1)
    DROP ASSEMBLY SqlServerSlackAPI
GO

/*
 *
 * Install new assembly to target database
 *
 */
CREATE ASSEMBLY SqlServerSlackAPI from 'C:\temp\dev\github\SqlServerSlackAPI\src\SqlServerSlackAPI\bin\debug\SqlServerSlackAPI.dll'  WITH PERMISSION_SET = EXTERNAL_ACCESS  
GO

/*
 *
 * Install procedures and functions
 *
 */
CREATE FUNCTION SlackChatPostMessage(
        @Token		nvarchar(max),
        @Channel		nvarchar(max),
        @Text		nvarchar(max),
        @UserName	nvarchar(max),
        @IconUrl		nvarchar(max)
    ) 
    RETURNS TABLE(
        Ok			bit,
        Channel		nvarchar(max),
        TimeStamp	nvarchar(max),
        Error		nvarchar(max)
    )
AS EXTERNAL NAME SqlServerSlackAPI.UserDefinedFunctions.SlackChatPostMessage;
GO

/*
 *
 * Test created functions and procedures
 *
 */

 SELECT Ok,
        Channel,
        TimeStamp,
        Error
    FROM dbo.SlackChatPostMessage(
        '<your slack token>',
        '@devlead',
        'Hello from SQL Server',
        null,
        null
    )