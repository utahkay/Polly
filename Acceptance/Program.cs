using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using Polly;
using Polly.CircuitBreaker;
using PS.Messaging;
using PS.MessagingAdmin;
using PS.Utilities.Serialization;

namespace Acceptance
{
    [TestFixture]
    public class Program
    {
        private MetadataApi api;
        protected const string TestUser = "kay-johansen";
        protected const string TestPassword = "testing1";
        private static int count;

        public static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            api = new MetadataApi("http://pluralsight.local/web2/metadata/live");
            var policy = Policy
                .Handle<Exception>()
                .CircuitBreaker(3, TimeSpan.FromSeconds(10));

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    policy.Execute(ConfigureQueues);

                }
                catch (BrokenCircuitException ex)
                {
                    Console.Out.WriteLine("The circuit is broken. Use a reasonable default.");
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine("Exception: " + ex.Message);
                }
            }

            Console.Out.WriteLine("Hit Enter to exit");
            Console.In.ReadLine();
        }

        private void FailThenSucceed()
        {
            Console.Out.WriteLine("Called");
            Thread.Sleep(1000);
            if (++count <= 3) 
                throw new InvalidOperationException("Gotcha!");
        }

        private void ConfigureQueues()
        {
            var exchangeName = new ExchangeName("test-org", "test-context", "test-event", 1);
            var queueName = new QueueName(exchangeName, "test-org", "another-context", "test-service");

            var busAdmin = new BusAdministrator(new BusAdministrationSettings(), new HttpClient(new JsonSerializer()));
            busAdmin.CreateExchange(exchangeName);
            busAdmin.BindQueueToExchange(queueName);
//            Console.Out.WriteLine(string.Join(",", busAdmin.BoundQueues(exchangeName)));
        }

        private void DoTheLogin()
        {
            var response = api.Login(TestUser, TestPassword);
            Assert.That(response.Token, Is.Not.Empty);
            Console.Out.WriteLine("Token is: " + response.Token);
        }
    }

    class BusAdministrationSettings : IBusAdministrationSettings
    {
        public string QueueManagerBaseUri
        {
            get { return "http://192.168.33.10:3000/"; }
        }
    }
}