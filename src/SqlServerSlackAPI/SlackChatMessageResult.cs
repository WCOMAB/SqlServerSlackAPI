using System.Text;

namespace SqlServerSlackAPI
{
    /// <summary>
    /// The result of Slack Chat API post
    /// </summary>
    public sealed class SlackChatMessageResult
    {
        private readonly bool _ok;
        private readonly string _channel;
        private readonly string _timeStamp;
        private readonly string _error;

        /// <summary>
        /// Indicating success or failure, <see cref="Error"/> for info on failure 
        /// </summary>
        public bool Ok
        {
            get { return _ok; }
        }

        /// <summary>
        /// Encoded channel ID
        /// </summary>
        public string Channel
        {
            get { return _channel; }
        }

        /// <summary>
        /// Timestamp of the message
        /// </summary>
        public string TimeStamp
        {
            get { return _timeStamp; }
        }

        /// <summary>
        /// Error message on failure
        /// </summary>
        public string Error
        {
            get { return _error; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ok">Indicating success or failure</param>
        /// <param name="channel">Encoded channel ID</param>
        /// <param name="timeStamp">Timestamp of the message</param>
        /// <param name="error">Error message on failure</param>
        public SlackChatMessageResult(bool ok, string channel, string timeStamp, string error)
        {
            _ok = ok;
            _channel = channel;
            _timeStamp = timeStamp;
            _error = error;
        }

        /// <summary>
        /// Converst this instance of value to a string representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append("{ Ok = ");
            builder.Append(Ok);
            builder.Append(", Channel = ");
            builder.Append(Channel);
            builder.Append(", TimeStamp = ");
            builder.Append(TimeStamp);
            builder.Append(", Error = ");
            builder.Append(Error);
            builder.Append(" }");
            return builder.ToString();
        }
    }
}