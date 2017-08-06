namespace Slack.Notification.Library
{
    /// <summary>
    /// To Build the slack message
    /// </summary>
    public class SlackMessage
    {
        /// <summary>
        /// Message to be sent to slack
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// Channel to which message to be sent
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// username that displayed as header on slack channel
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Slack message Icon
        /// </summary>
        public string SlackEmoji { get; set; }
    }
}
