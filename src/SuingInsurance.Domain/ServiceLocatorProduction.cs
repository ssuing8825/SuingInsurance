using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NEventStore;
using NEventStore.Persistence.Sql.SqlDialects;

using SuingInsurance.Domain.DataLayer;

namespace SuingInsurance.Domain
{
    public class ServiceLocatorProduction : ServiceLocatorBase
    {
        private IStoreEvents eventstore;

        protected override IStoreEvents CreateEventStreamCore()
        {
            if (eventstore == null)
            {
                eventstore = Wireup.Init()
                        .UsingSqlPersistence("EventStore")
                        .WithDialect(new MsSqlDialect())
                        .InitializeStorageEngine()
                        .UsingJsonSerialization()
                        .Compress()
                    // .EncryptWith(EncryptionKey)
                    // .HookIntoPipelineUsing(new[] { new AuthorizationPipelineHook() })
                    .UsingSynchronousDispatchScheduler()
                      .DispatchTo(new AzureServiceBusDispatcher())
                        .Build();
            }

            return eventstore;
        }

        /// <summary>
        /// Method to create ClientManagerCore.
        /// </summary>
        /// <returns>Returns ClientManagerBase.</returns>
        protected override PolicyDataManager CreatePolicyDataManagerCore()
        {
            return new PolicyDataManager(this);
        }
    }
}