using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.S3Events;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Slack.Notification.Library;

namespace AWS.S3Notification.Lambda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task TestS3EventLambdaFunction()
        {
            // Use your Amazon awsAccessKeyId, awsAccessSecretId, RegionEndPoint
            string awsAccessKeyId = "";
            string awsAccessSecretId = "";
            IAmazonS3 s3Client = new AmazonS3Client(awsAccessKeyId, awsAccessSecretId, RegionEndpoint.USEast1);

            var bucketName = "lambda-AWS.S3Notification.Lambda-".ToLower() + DateTime.Now.Ticks;
            var key = "text.json";

            // Create a bucket an object to setup a test data.
            await s3Client.PutBucketAsync(bucketName);
            try
            {
                await s3Client.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    ContentBody = "{event:'s3event', company:'amazon', 'description':'Capturing S3 event notification using Lambda'}"
                });

                // Setup the S3 event object that S3 notifications would create with the fields used by the Lambda function.
                var s3Event = new S3Event
                {
                    Records = new List<S3EventNotification.S3EventNotificationRecord>
                    {
                        new S3EventNotification.S3EventNotificationRecord
                        {
                            S3 = new S3EventNotification.S3Entity
                            {
                                Bucket = new S3EventNotification.S3BucketEntity {Name = bucketName },
                                Object = new S3EventNotification.S3ObjectEntity {Key = key }
                            }
                        }
                    }
                };

                // Invoke the lambda function and confirm the content type was returned.
                //Create webhook url from slack website https://api.slack.com/incoming-webhooks and replace below url string with one you have acsess
                string url = "https://hooks.slack.com/services/T00000000/B00000000/XXXXXXXXXXXXXXXXXXXXXXXX";
                var webhookUrl = new Uri(url);
                var function = new AWSS3EventNotification(s3Client, new SlackClient(), webhookUrl, BuildSlackMessage());
                ILambdaContext ctx = new TestLambdaContext();
                var contentType = await function.FunctionHandler(s3Event, ctx);
                Assert.Equal("Handler: Got Data from Key text.json for s3event with description Capturing S3 event notification using Lambda", contentType);
            }
            finally
            {
                // Clean up the test data
               await AmazonS3Util.DeleteS3BucketWithObjectsAsync(s3Client, bucketName);
            }
        }
        private SlackMessage BuildSlackMessage()
        {
            //build the slack message you want to send in case of S3 event trigger
            return new SlackMessage
            {
                UserName = "AWSS3Event",
                Channel = "slack channel name",
                Message = "S3 Event Notification",
                SlackEmoji = ":information_source:"
            };
        }
    }
}
