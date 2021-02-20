using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceBusTopic
{
    internal static class Program
    {
        private static async Task Main()
        {
            //https://stackoverflow.com/questions/58453707/azure-topics-multiple-listeners-on-same-subscription
            var topicService = new TopicService();
            
            await topicService.ReceiveMessagesFromSubscriptionAsync("all");
            await topicService.ReceiveMessagesFromSubscriptionAsync("cats");
            await topicService.ReceiveMessagesFromSubscriptionAsync("dogs");

            await SendMessage(topicService);
            await SendBatch(topicService);
            
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        private static async Task SendMessage(TopicService topicService)
        {
            var payload = new Payload(new List<int> {100, 200, 300}, Type.Cat);
            await topicService.SendMessageAsync(payload);
        }
        
        private static async Task SendBatch(TopicService topicService)
        {
            var payloads = new Queue<Payload>();
            payloads.Enqueue(new Payload(new List<int> {9, 8, 7}, Type.Cat));
            payloads.Enqueue(new Payload(new List<int> {4, 5, 6}, Type.Dog));
            payloads.Enqueue(new Payload(new List<int> {1, 2, 3}, Type.Dog));
            await topicService.SendBatchAsync(payloads);
        }
    }
}