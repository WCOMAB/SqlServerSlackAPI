using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using SqlServerSlackAPI.LitJson;

namespace SqlServerSlackAPI
{
    internal static class SlackChatApi
    {
        internal static SlackChatMessageResult PostMessage(
            string token,
            string channel,
            string text,
            string userName,
            string iconUrl
            )
        {
            const string postMessageUri = "https://slack.com/api/chat.postMessage";
            NameValueCollection messageParams;
            string error;
            if (!TryGetMessageParams(
                token,
                channel,
                text,
                userName,
                iconUrl,
                out messageParams,
                out error
                ))
            {
                return new SlackChatMessageResult(false, null, null, error);
            }

            var result = PostToChatApi(
                postMessageUri,
                messageParams
                );

           
            return result;
        }


        private static SlackChatMessageResult PostToChatApi(
            string apiUri,
            NameValueCollection apiParameters
            )
        {
            using (var client = new WebClient())
            {
                var resultBytes = client.UploadValues(
                    apiUri,
                    apiParameters
                    );
                var resultJson = Encoding.UTF8.GetString(
                    resultBytes
                    );


                var result = JsonMapper.ToObject(resultJson);
                var parsedResult = new SlackChatMessageResult(
                    GetBoolean(result,"ok") ?? false,
                    GetString(result,"channel"),
                    GetString(result,"ts"),
                    GetString(result,"error")
                    );
                return parsedResult;
            }
        }

        private static bool TryGetMessageParams(
            string token,
            string channel,
            string text,
            string userName,
            string iconUrl,
            out NameValueCollection messageParams,
            out string error
            )
        {
            messageParams = null;
            if (string.IsNullOrWhiteSpace(token))
            {
                error = string.Format("Invalid Message Token specified ({0})", (token ?? "NULL"));
                return false;
            }

            if (string.IsNullOrWhiteSpace(channel))
            {
                error = string.Format("Invalid Message Channel specified ({0})", (channel ?? "NULL"));
                return false;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                error = string.Format("Invalid Message Text specified ({0})", (text ?? "NULL"));
                return false;
            }

            messageParams = new NameValueCollection
            {
                {"token", token},
                {"channel", channel},
                {"text", text},
                {"username", userName ?? "WCOM SQL Server Slack API"},
                {
                    "icon_url",
                    iconUrl ?? "https://zapier.cachefly.net/storage/services/0401f13c0793fb7fefd20e84b93f8d96.64x64.png"
                }
            };
            error = null;
            return true;
        }

        private static string GetString(JsonData data, string key)
        {
            return (data != null && data.Keys.Contains(key))
                ? (string) data[key]
                : null;
        }

        private static bool? GetBoolean(JsonData data, string key)
        {
            return (data != null && data.Keys.Contains(key))
                ? (bool) data[key]
                : null as bool?;
        }
    }
}
