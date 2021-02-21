using System.Collections.Generic;

namespace EventGridTest
{
    internal static class Program
    {
        private static void Main()
        {
            var eventGridService = new EventGridService();

            var payloads = new List<Payload>
            {
                new Payload(new List<int> {9, 8, 7}, Type.Cat),
                new Payload(new List<int> {4, 5, 6}, Type.Dog),
                new Payload(new List<int> {1, 2, 3}, Type.Dog)
            };
            
            eventGridService.SendEvent(payloads);
        }
    }
}