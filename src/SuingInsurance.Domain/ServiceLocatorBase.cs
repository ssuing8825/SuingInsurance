using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEventStore;

using SuingInsurance.Domain.DataLayer;

namespace SuingInsurance.Domain
{
    public abstract class ServiceLocatorBase
    {
        public IStoreEvents CreateEventStream()
        {
            return this.CreateEventStreamCore();
        }

        public PolicyDataManager CreatePolicyDataManagerManager()
        {
            return this.CreatePolicyDataManagerCore();
        }

        protected abstract IStoreEvents CreateEventStreamCore();
        protected abstract PolicyDataManager CreatePolicyDataManagerCore();

    }
}
