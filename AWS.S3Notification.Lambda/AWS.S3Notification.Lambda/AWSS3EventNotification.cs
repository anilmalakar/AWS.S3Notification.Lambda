using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon.Runtime.Internal.Util;
using Amazon.S3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Slack.Notification.Library;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace AWS.S3Notification.Lambda
{
    public class AWSS3EventNotification
    {
        private readonly IAmazonS3 _amazonS3Client;
        private readonly ISlackClient _slackClient;
        private readonly Uri _webHookUri;
        private readonly SlackMessage _message;

        private static Logger _log = Logger.GetLogger(typeof(AWSS3EventNotification));
        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public AWSS3EventNotification() : this(new AmazonS3Client(), new SlackClient(), new Uri(""), new SlackMessage())
        {
            _log.DebugFormat("In S3EventNotification default constructor");
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="amazonS3Client"></param>
        /// <param name="slackClient"></param>
        /// <param name="webHookUri"></param>
        /// <param name="message"></param>
        public AWSS3EventNotification(IAmazonS3 amazonS3Client, ISlackClient slackClient, Uri webHookUri, SlackMessage message)
        {
            _amazonS3Client = amazonS3Client;
            _slackClient = slackClient;
            _webHookUri = webHookUri;
            _message = message;

            _log.DebugFormat("In S3EventNotification parameterized constructor, initialized S3 client instance");
        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="s3Event"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(S3Event s3Event, ILambdaContext context)
        {
            
            context.Logger.LogLine("S3 Event Function Handler");
            var s3CurrentEvent = s3Event.Records?[0].S3;
            if (s3CurrentEvent == null)
                return null;
            try
            {
                string eventMessage;
                context.Logger.LogLine("Handler: Trying to GetObjectAsync from Amazon S3 Client");
                // Get the S3 object that trigger the event
                var response = await _amazonS3Client.GetObjectAsync(s3CurrentEvent.Bucket.Name, s3CurrentEvent.Object.Key);
                // read the response stream and  write your business logic what you want to do with event trigerred and data received.
                using (var stream = response.ResponseStream)
                {
                    var reader = new StreamReader(stream);
                    var s3Document = reader.ReadToEnd();
                    reader.Dispose();
                    var jsonDoc = JsonConvert.DeserializeObject(s3Document);
                    var items = JObject.Parse(jsonDoc.ToString());
                    eventMessage =
                        ($"Handler: Got Data from Key {s3CurrentEvent.Object.Key} for {items.SelectToken("event").Value<string>()}" +
                         $" with description {items.SelectToken("description").Value<string>()}");
                    context.Logger.LogLine(eventMessage);
                }

                // you can modify the SlackMessage message here, before sending it.
                await _slackClient.SendMessageAsync(_message, _webHookUri);
                context.Logger.LogLine($"Handler: Sent notification to channel {_message.Channel}");
                return eventMessage;
            }
            catch (Exception e)
            {
                context.Logger.LogLine($"Error getting object {s3CurrentEvent.Object.Key} from bucket {s3CurrentEvent.Bucket.Name}. Make sure they exist and your bucket is in the same region as this function.");
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                throw;
            }
        }
    }
}
