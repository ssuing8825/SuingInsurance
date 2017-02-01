using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SuingInsurance.Domain.Events;
using SuingInsurance.Domain.InputCommands;

using EventMessage = NEventStore.EventMessage;

namespace SuingInsurance.Domain.DataLayer
{
    public class PolicyDataManager
    {
        private ServiceLocatorBase serviceLocator;

        private const string BucketId = "DefaultBucket";

        public PolicyDataManager(ServiceLocatorBase serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public void CreateStreamAndFirstEvent(EventBase createdEvent)
        {
            var store = this.serviceLocator.CreateEventStream();

            // some business code here
            using (var stream = store.CreateStream(BucketId, createdEvent.PolicyNumber))
            {
                stream.Add(new EventMessage { Body = createdEvent });
                stream.CommitChanges(createdEvent.MessageId);
            }
        }

        public void AppendToStream(EventBase createdEvent)
        {
            var store = this.serviceLocator.CreateEventStream();

            // some business code here
            using (var stream = store.OpenStream(BucketId, createdEvent.PolicyNumber, 0, int.MaxValue))
            {
                stream.Add(new EventMessage { Body = createdEvent });
                stream.CommitChanges(createdEvent.MessageId);
            }
        }

        public ICollection<EventMessage> GetPolicyEvents(string policyNumber)
        {
            var store = this.serviceLocator.CreateEventStream();
            var policyEvents = new List<object>();

            // some business code here
            using (var stream = store.OpenStream(BucketId, policyNumber, 0, int.MaxValue))
            {
                return stream.CommittedEvents;
            }
        }
    }
}
