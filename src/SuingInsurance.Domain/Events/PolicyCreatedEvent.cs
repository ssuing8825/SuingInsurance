using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SuingInsurance.Domain.Events
{
    [KnownType(typeof(PolicyCreatedEvent))]
    [KnownType(typeof(CoverageAddedEvent))]
    public abstract class EventBase
    {
        public Guid MessageId { get; set; }
        public DateTime DateOfEvent { get; set; }
        public string PolicyNumber { get; set; }
    }

    public class PolicyCreatedEvent : EventBase
    {
        public List<string> Coverage { get; set; }
    }

    public class CoverageAddedEvent : EventBase
    {
        public string Coverage { get; set; }
    }
}
