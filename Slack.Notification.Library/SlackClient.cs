using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Slack.Notification.Library
{
    /// <summary>
    /// Slack client is use for bulding and sending message to slack channel
    /// </summary>
    public class SlackClient : ISlackClient
    {
        /// <summary>
        /// slack channel uri
        /// </summary>
        private Uri _webhookUrl;
        /// <summary>
        /// HttpClient object to post message
        /// </summary>
        private readonly HttpClient _httpClient;

        public SlackClient()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// SendMessageAsync to send message using httpclient
        /// </summary>
        /// <param name="slackMessage"></param>
        /// <param name="webhookUrl"></param>
        /// <returns>http response</returns>
        public async Task<HttpResponseMessage> SendMessageAsync(SlackMessage slackMessage, Uri webhookUrl)
        {
            _webhookUrl = webhookUrl;
            var payLoad = BuildSlackMessage(slackMessage);
            var response = await _httpClient.PostAsync(_webhookUrl,
                                            new StringContent(payLoad.ToString(), 
                                            Encoding.UTF8,
                                            "application/json"));
            _httpClient.Dispose();
            return response;
        }

        /// <summary>
        /// Build the slack message
        /// </summary>
        /// <param name="slackMessage"></param>
        /// <returns>JObject</returns>
        private JObject BuildSlackMessage(SlackMessage slackMessage)
        {
            return new JObject
            {
                {"channel", slackMessage.Channel},
                {"text", slackMessage.Message},
                {"username", slackMessage.UserName},
                {"icon_emoji", slackMessage.SlackEmoji}
            };
        }
    }
}
