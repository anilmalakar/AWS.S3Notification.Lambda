using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Slack.Notification.Library
{
    /// <summary>
    /// interface to define contract for slack client
    /// </summary>
    public interface ISlackClient
    {
        Task<HttpResponseMessage> SendMessageAsync(SlackMessage slackMessage, Uri webhookUrl);
    }
}