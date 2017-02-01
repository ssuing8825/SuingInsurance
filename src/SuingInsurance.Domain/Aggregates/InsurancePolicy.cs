using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using SuingInsurance.Domain.DataLayer;
using SuingInsurance.Domain.Events;
using SuingInsurance.Domain.InputCommands;

namespace SuingInsurance.Domain.Aggregates
{
    public class InsurancePolicy
    {
        [JsonIgnore]
        private readonly ServiceLocatorBase serviceLocator;

        [JsonIgnore]
        private PolicyDataManager policyDataManager;

        public DateTime EffetiveDate { get;  set; }

        public List<string> Coverage { get;  set; }

        public string PolicyNumber { get; set; }

        public int Version { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id
        {
            get
            {
                return PolicyNumber + "_" + Version;
            }
            set
            {
                PolicyNumber = value.Substring(0, value.IndexOf('_'));
                Version = int.Parse(value.Substring(value.IndexOf('_') + 1, value.Length - (value.IndexOf('_') + 1)));
            }
        }

        public InsurancePolicy(ServiceLocatorBase serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }
        private InsurancePolicy()
        {
        }
        
        public void GetPolicyByPolicyNumber(string policyNumber)
        {
            var policyEvents = this.PolicyData.GetPolicyEvents(policyNumber);

            //This is a poormans router, but shows that you need to inflate from events
            foreach (var eventMessage in policyEvents)
            {
                ApplyEvent(eventMessage.Body)
              ;
            }
        }

        public void ApplyEvent(object @event)
        {
            if (@event is PolicyCreatedEvent)
            {
                this.ApplyEvent((PolicyCreatedEvent)@event);
            }
            if (@event is CoverageAddedEvent)
            {
                this.ApplyEvent((CoverageAddedEvent)@event);
            }
        }

        public void AddCoverageToPolicy(AddCoverageCommand addCoverageCommand)
        {
            var @event = new CoverageAddedEvent()
                                         {
                                             PolicyNumber = this.PolicyNumber,
                                             MessageId = Guid.NewGuid(),
                                             Coverage = addCoverageCommand.Coverage
                                         };

            this.ApplyEvent(@event);

            this.PolicyData.AppendToStream(@event);
        }

        public void CreatePolicy(CreatePolicyCommand createPolicyCommand)
        {
            var @event = new PolicyCreatedEvent()
            {
                PolicyNumber = createPolicyCommand.PolicyNumber,
                MessageId = Guid.NewGuid(),
                Coverage = createPolicyCommand.Coverage

            };

            this.ApplyEvent(@event);
            this.PolicyData.CreateStreamAndFirstEvent(@event);
        }
      
        private void ApplyEvent(PolicyCreatedEvent @event)
        {
            this.Coverage = new List<string>();
            this.Coverage.AddRange(@event.Coverage);
            this.EffetiveDate = DateTime.Now;
            this.Coverage = @event.Coverage;
            this.PolicyNumber = @event.PolicyNumber;
            this.Version = 1;

        }

        private void ApplyEvent(CoverageAddedEvent @event)
        {
            this.Coverage.Add(@event.Coverage);
            this.Version++;
        }

        /// <summary>
        /// Gets the client service gateway
        /// </summary>
        [JsonIgnore]
        private PolicyDataManager PolicyData
        {
            get
            {
                return this.policyDataManager ?? (this.policyDataManager = this.serviceLocator.CreatePolicyDataManagerManager());
            }
        }

    }
}
