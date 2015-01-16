using System.Collections;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using SqlServerSlackAPI;


// ReSharper disable CheckNamespace
public class UserDefinedFunctions
{
    [SqlFunction(FillRowMethodName = "SlackChatPostMessageFillRow")]
    public static IEnumerable SlackChatPostMessage(
        string token,
            string channel,
            string text,
            string userName,
            string iconUrl
        )
    {
        yield return SlackChatApi.PostMessage(
            token,
            channel,
            text,
            userName,
            iconUrl
            );
    }

    public static void SlackChatPostMessageFillRow(
        object value,
        out SqlBoolean ok,
        out SqlString channel,
        out SqlString timeStamp,
        out SqlString error

        )
    {
        var result = value as SlackChatMessageResult;
        if (result == null)
        {
            ok = SqlBoolean.Null;
            channel = SqlString.Null;
            timeStamp = SqlString.Null;
            error = SqlString.Null;
        }
        else
        {
            ok = result.Ok;
            channel = result.Channel;
            timeStamp = result.TimeStamp;
            error = result.Error;
        }
    }
}

