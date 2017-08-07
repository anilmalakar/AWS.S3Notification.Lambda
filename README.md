# AWS.S3Notification.Lambda
AWS S3 Slack Notification using AWS Lambda in C#,.Net Core
This solution has been written to send S3 event notification [Object creation or any] to notify user on Slack. To use this solution use download solution and use below steps.
1. Use Visual Studio 2015 to open solution and restore nuget packages.
2. Make sure you have .NET core tools and AWS plugin installed for Visual Studio.
3. Build the solution.
4. Go to AWS.S3Notification.Lambda.Tests project and open FunctionTest class
5. Provide your Amazon awsAccessKeyId, awsAccessSecretId, RegionEndPoint to connect to AWS.
6. Create webhook url from slack website https://api.slack.com/incoming-webhooks and replace below url string with one you have acsess
   string url = "https://hooks.slack.com/services/T00000000/B00000000/XXXXXXXXXXXXXXXXXXXXXXXX";
7. Add the required details in BuildSlackMessage like UserName, Slack channel where you want to send S3 event notification and Message to be sent.
8. Debug the test and see the slack notification once S3 event executed.
9. You can use similar approach to create a client for your AWS Lambda function and can capture the S3 events.
10. You need not to fire S3 event from code for your practical use but rather configure Lamda in AWS for S3 bucket you want to caputre the event for.


