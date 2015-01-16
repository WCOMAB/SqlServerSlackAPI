# SqlServerSlackAPI

SqlServerSlackAPI is a SQL Server CLR assembly for direct communication with Slack via TSQL

![Slack message from SQL Server](https://raw.githubusercontent.com/WCOMAB/SqlServerSlackAPI/master/images/sql2slack.png)


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


