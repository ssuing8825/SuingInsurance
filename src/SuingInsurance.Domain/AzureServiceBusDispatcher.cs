using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

using NEventStore.Dispatcher;
using NEventStore.Persistence;

namespace SuingInsurance.Domain
{
    public class AzureServiceBusDispatcher : IDispatchCommits
    {
        private TopicClient client;

        public AzureServiceBusDispatcher()
        {
            client = TopicClient.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"], "PolicyEvents");
        }

        public void Dispatch(NEventStore.ICommit commit)
        {
            foreach (var @event in commit.Events)
            {
                // Create message, passing a string message for the body.
                BrokeredMessage message = new BrokeredMessage(@event.Body);

                // Set additional custom app-specific property.
                message.Properties["EventName"] = @event.Body.GetType().Name;
                message.Properties.Add("StreamRevision", commit.StreamRevision);
                this.client.Send(message);
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
