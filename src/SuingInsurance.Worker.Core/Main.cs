using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ServiceBus.Messaging;

using SuingInsurance.Domain.Aggregates;
using SuingInsurance.Domain.Events;

using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;


namespace SuingInsurance.Worker.Core
{
    public class Main
    {
        private static string connectionString;
        private const string EndpointUri = "https://suinginsurance.documents.azure.com:443/";
        private const string PrimaryKey = "ppvbauxXfa1WusXLKSn2zXwJDZPPlChSsYFFOHfJ9uRWjvfb8Dv6trjdfUXFSxFZz0yUFWkCBbKQ6w5qcg6AlA==";
        private DocumentClient documentClient;

        private string databaseName = "SuingInsurance";
        private string collectionName = "InsurancePolicy";


        public Main()
        {
            connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            this.documentClient = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
        }

        public async Task Start()
        {
            SubscriptionClient coverageAddedClient = SubscriptionClient.CreateFromConnectionString(connectionString, "PolicyEvents", "CoverageAddedEvent");
            SubscriptionClient policyCreatedClient = SubscriptionClient.CreateFromConnectionString(connectionString, "PolicyEvents", "PolicyCreatedEvent");

            Task recieveCoverageAddedTask = coverageAddedClient.ReceiveAsync().ContinueWith(ProcessCoverageAddedMessage);
            Task recievePolicyCreateTask = policyCreatedClient.ReceiveAsync().ContinueWith(ProcessPolicyCreatedMessage);

            Task.WaitAll(recieveCoverageAddedTask, recievePolicyCreateTask);

            Console.WriteLine("All messages received");

        }

        private async void ProcessCoverageAddedMessage(Task<BrokeredMessage> t)
        {
            BrokeredMessage message = t.Result;

            try
            {
                // Process message from subscription.
                Console.WriteLine("\n**Coverage Added**");

                var @event = message.GetBody<CoverageAddedEvent>();
                Console.WriteLine("Body: " + @event.PolicyNumber);
                Console.WriteLine("StreamRevision: " + message.Properties["StreamRevision"]);

                int revisionNumber = (int)message.Properties["StreamRevision"];
                var policy = this.RetrievePolicy(@event.PolicyNumber + "_" + (revisionNumber - 1));
                policy.ApplyEvent(@event);

                CreateVersionOfPolicy(policy);

                // Remove message from subscription.
                await message.CompleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Indicates a problem, unlock message in subscription.
                message.Abandon();
            }


        }

        private async void ProcessPolicyCreatedMessage(Task<BrokeredMessage> t)
        {
            BrokeredMessage message = t.Result;
            try
            {
                // Process message from subscription.
                Console.WriteLine("\n**PolicyIssued**");

                var @event = message.GetBody<PolicyCreatedEvent>();
                Console.WriteLine("Body: " + @event.PolicyNumber);
                Console.WriteLine("StreamRevision: " + message.Properties["StreamRevision"]);
                var policy = new InsurancePolicy(null); //Yuck!
                policy.ApplyEvent(@event);
                CreateVersionOfPolicy(policy);

                // Remove message from subscription.
                await message.CompleteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                // Indicates a problem, unlock message in subscription.
                message.Abandon();
            }
        }

        private void CreateVersionOfPolicy(InsurancePolicy policy)
        {
            this.documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(this.databaseName, this.collectionName), policy).Wait();
            this.WriteToConsoleAndPromptToContinue("Created Policy with key {0}", policy.Id);
        }

        private InsurancePolicy RetrievePolicy(string id)
        {
            var policies = this.documentClient.CreateDocumentQuery<InsurancePolicy>(
                 UriFactory.CreateDocumentCollectionUri(databaseName, collectionName),
                 String.Format("SELECT * FROM InsurancePolicy WHERE InsurancePolicy.id = '{0}'", id)).ToList();

            return policies[0];

        }


        private void WriteToConsoleAndPromptToContinue(string format, params object[] args)
        {
            Console.WriteLine(format, args);
            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }
    }
}
