using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Messaging.ServiceBus;

namespace ServiceBusTopic
{
    public class TopicService
    {
        private const string Namespace = "andreas-asdfgdfgfd.servicebus.windows.net";
        private const string TopicName = "updates";
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;

        public TopicService()
        {
            
            _client = new ServiceBusClient(Namespace, new DefaultAzureCredential());
            _sender = _client.CreateSender(TopicName);
        }

        public async Task SendMessageAsync(Payload payload)
        {
            await _sender.SendMessageAsync(CreateMessage(payload));
            Console.WriteLine($"Sent a single message to the topic: {TopicName}");
        }
        
        public async Task SendBatchAsync(Queue<Payload> payloads)
        {
            var payloadCount = payloads.Count;
            while (payloads.Count > 0)
            {
                using var messageBatch = await _sender.CreateMessageBatchAsync();

                if (messageBatch.TryAddMessage(CreateMessage(payloads.Peek())))
                {
                    payloads.Dequeue();
                }
                else
                {
                    throw new Exception($"Message {payloadCount - payloads.Count} is too large and cannot be sent.");
                }

                while (payloads.Count > 0 && messageBatch.TryAddMessage(CreateMessage(payloads.Peek())))
                {
                    payloads.Dequeue();
                }
                
                await _sender.SendMessagesAsync(messageBatch);
            }

            Console.WriteLine($"Sent a batch of {payloadCount} messages to the topic: {TopicName}");
        }
        
        public async Task ReceiveMessagesFromSubscriptionAsync(string subscriptionName)
        {
            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 2,
                ReceiveMode = ServiceBusReceiveMode.PeekLock
            };
            var processor = _client.CreateProcessor(TopicName, subscriptionName, options);
            processor.ProcessMessageAsync += args => MessageHandler(args, subscriptionName);
            processor.ProcessErrorAsync += args => ErrorHandler(args, subscriptionName);
            await processor.StartProcessingAsync();
        }
        
        private static async Task MessageHandler(ProcessMessageEventArgs args, string subscription)
        {
            var message = args.Message;
            Console.WriteLine($"Received: {message.Body}, id {message.MessageId}, subscription {subscription}");
            await args.CompleteMessageAsync(message);
        }
        
        private static Task ErrorHandler(ProcessErrorEventArgs args, string subscription)
        {
            Console.WriteLine($"Received error on subscription {subscription}");
            Console.WriteLine(args.ErrorSource);
            Console.WriteLine(args.FullyQualifiedNamespace);
            Console.WriteLine(args.EntityPath);
            Console.WriteLine(args.Exception.ToString());
            return Task.CompletedTask;
        }
        
        private static ServiceBusMessage CreateMessage(Payload payload)
        {
            var message = new ServiceBusMessage(payload.ToJson);
            message.ApplicationProperties.Add("type",payload.Type);
            return message;
        }
    }
}
