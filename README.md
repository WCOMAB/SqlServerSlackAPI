# SqlServerSlackAPI

SqlServerSlackAPI is a SQL Server CLR assembly for direct communication with Slack via TSQL

[![Build status](https://ci.appveyor.com/api/projects/status/73tb29x0ii0amrru?svg=true)](https://ci.appveyor.com/project/WCOMAB/sqlserverslackapi)
<br/>

![Slack message from SQL Server](https://raw.githubusercontent.com/WCOMAB/SqlServerSlackAPI/master/images/sql2slack.png)

##Table of contents
1. [Usage](https://github.com/WCOMAB/SqlServerSlackAPI#usage)
    * [SlackChatPostMessage](https://github.com/WCOMAB/SqlServerSlackAPI#slackchatpostmessage)
        * [Parameters](https://github.com/WCOMAB/SqlServerSlackAPI#parameters)
        * [Example](https://github.com/WCOMAB/SqlServerSlackAPI#resultset)
2. [Compiling](https://github.com/WCOMAB/SqlServerSlackAPI#compiling)
   * [Prerequisites](https://github.com/WCOMAB/SqlServerSlackAPI#prerequisites)
   * [Signing](https://github.com/WCOMAB/SqlServerSlackAPI#signing)
      * [Creating your own test cert via command line](https://github.com/WCOMAB/SqlServerSlackAPI#creating-your-own-test-cert-via-command-line)  
      * [Creating your own cert via Visual Studio](https://github.com/WCOMAB/SqlServerSlackAPI#creating-your-own-cert-via-visual-studio)
   * [CI /CAKE](https://github.com/WCOMAB/SqlServerSlackAPI#ci--cake)
3. [Deployment](https://github.com/WCOMAB/SqlServerSlackAPI#deployment)

## Usage

### SlackChatPostMessage

Table value function that posts message via HTTP to Slack Channel API and parses json response.

#### Parameters

| Name        | Type            | Required | Description                                    |
|-------------|-----------------|----------|------------------------------------------------|
| `@Token`    | nvarchar(_max_) | yes      | Slack auth token                               |
| `@Channel`  | nvarchar(_max_) | yes      | Channel to send message to. Can be a public channel, private group or IM channel. Can be an encoded ID, or a name. |
| `@Text`     | nvarchar(_max_) | yes      | Text of the message to send. For message formatting see https://api.slack.com/docs/formatting |
| `@UserName` | nvarchar(_max_) | no       | Name of bot.                                   |
| `@IconUrl`  | nvarchar(_max_) | no       | URL to an image to use as the icon for the bot |

#### Example

```sql
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
```

#### Resultset

| Name        | Type            | Description                                               |
|-------------|-----------------|-----------------------------------------------------------|
| `Ok`        | bit             | Indicating success/failure, see `Error` for error details |
| `Channel`   | nvarchar(_max_) | Encoded channel ID                                        |
| `TimeStamp` | nvarchar(_max_) | Timestamp of the message                                  |
| `Error`     | nvarchar(_max_) | Error message on failure                                  |

On success the table value funcion will return something like

| Ok | Channel   | TimeStamp         | Error |
|----|-----------|-------------------|-------|
| 1  | D03A9F0Q5 | 1421363019.000005 | NULL  |

On failure the you could get an error / exception for unhandled errors, but for most errors you will get an resonse like below where `Error` column contains details about the issue.

| Ok | Channel   | TimeStamp         | Error        |
|----|-----------|-------------------|--------------|
| 0  | NULL      | NULL              | invalid_auth |


## Compiling

### Prerequisites
1. Visual Studio 2013 Update 4
2. Latest version of SQL Server database tooling (https://msdn.microsoft.com/en-us/dn864412)

### Signing

Signing is currently disabled in the project, but enabled in the build script.
A test cert is supplied in the repository (_.\src\SqlServerSlackAPI\SqlServerSlackAPI.pfx_) it's password is `SqlServerSlackAPI`

#### Creating your own test cert via command line

Using command line this is the process for created self sign cert (preferably for production you've bough an real cert, but for testing this is just fine)

First use `makecert`, it's included in the Windows SDK usually located `C:\Program Files (x86)\Windows Kits\[sdk version]\bin\[cpu]` i.e. `C:\Program Files (x86)\Windows Kits\8.1\bin\x64` 

Creating the private key & cert used to create pfx is done like this
```powershell
&"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\makecert.exe" -n "CN=WCOM AB" -cy authority -a sha512 -sv "privatekey.pvk" -r thecert.cer
```
It will ask you for a password if you don't want one just press ok.

To combine private key & cert into a pfx we use the `pvk2pfx` tool found in same folder as `makecert`
You call `pvk2pfx` like this
```powershell
&"C:\Program Files (x86)\Windows Kits\8.1\bin\x64\pvk2pfx.exe" -pvk .\privatekey.pvk -spc .\thecert.cer -pfx privatekeyandcert.pfx
```
You will now have a PFX file `privatekeyandcert.pfx` you can use for signing.

#### Creating your own cert via Visual Studio
In Visual Studio, go to Solution Explorer, right click on Project -> Properties
In Properties, click on SQLCLR tab, then  `Signing...` button
Enable checkbox `Sign the assembly`, then in the dropdown choose `New...`,
In `Create Strong Name Key` put some key file name and then enter some password, then click `OK` button.

### CI / CAKE

For continuous integration scenarios CAKE build script is supplied. CAKE is bootstrapped via the `build.ps1` PowerShell script, which will fetch all dependencies and trigger MSBuild.

## Deployment

1. Build solution
2. Copy the result assembly to target SQL Server
3. Open [Deploy.sql](https://github.com/WCOMAB/SqlServerSlackAPI/blob/master/src/SqlServerSlackAPI/Deploy.sql) in Microsoft SQL Managment Studio
    * Replace `SlackTestDb` with the database you want to install the function in.
    * Replace `C:\temp\dev\github\SqlServerSlackAPI\src\SqlServerSlackAPI\bin\debug\SqlServerSlackAPI.dll` with the path on server where you copied the dll.
4. Execute script

You should then see

| Ok | Channel   | TimeStamp         | Error        |
|----|-----------|-------------------|--------------|
| 0  | NULL      | NULL              | invalid_auth |

You can then test with (adjusting to use your token & channel avail on your slack)
```sql
 SELECT Ok,
        Channel,
        TimeStamp,
        Error
    FROM dbo.SlackChatPostMessage(
        '<your slack token>',
        '#yourslackchannel',
        'Hello from SQL Server',
        null,
        null
    )
```

