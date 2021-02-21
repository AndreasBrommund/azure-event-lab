using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Identity;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

namespace EventGridTest
{
    public class EventGridService
    {
        private const string TopicEndpoint = "https://updates.northeurope-1.eventgrid.azure.net/api/events";
        private const string TopicKey = "";
        private readonly string _topicHostname = new Uri(TopicEndpoint).Host;
        private readonly EventGridClient _client;

        public EventGridService()
        {
            new DefaultAzureCredential();
            var topicCredentials = new TopicCredentials(TopicKey);
            _client = new EventGridClient(topicCredentials);
        }

        public void SendEvent(IEnumerable<Payload> payloads)
        {
            _client.PublishEventsAsync(_topicHostname, CreateEvent(payloads)).GetAwaiter().GetResult();
            Console.Write("Published events to Event Grid.");
        }

        private static IList<EventGridEvent> CreateEvent(IEnumerable<Payload> payloads)
        {
            return payloads.Select(p =>
                new EventGridEvent
                {
                    Data = p.ToJson,
                    DataVersion = "1.0",
                    EventTime = DateTime.Now,
                    EventType = p.Type,
                    Id = Guid.NewGuid().ToString(),
                    Subject = $"Update {p.Type}s"
                }).ToList();
        }
    }
}