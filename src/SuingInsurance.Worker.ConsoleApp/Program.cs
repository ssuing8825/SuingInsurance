using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

using SuingInsurance.Domain.Events;
using SuingInsurance.Worker.Core;

namespace SuingInsurance.Worker.ConsoleApp
{
    class Program
    {
        private static string connectionString;

        static void Main(string[] args)
        {
            connectionString = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            //CreateSubscriptions();

            //return;

            try
            {
                var mainapp = new Main();
                  //This is blocked from inside
                mainapp.Start();

                Console.WriteLine("I should be blocking");

            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                Console.WriteLine("Error: {0}, Message: {1}", e.Message, baseException.Message);
            }
            finally
            {
                Console.WriteLine("End of demo, press any key to exit.");
                Console.ReadKey();
            }
        }

        private static void CreateSubscriptions()
        {

            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

            SqlFilter policyCreatedFilter = new SqlFilter(string.Format("EventName = '{0}'", typeof(PolicyCreatedEvent).Name));
            namespaceManager.CreateSubscription("policyevents", "PolicyCreatedEvent", policyCreatedFilter);

            SqlFilter coverageAddedFilter = new SqlFilter(string.Format("EventName = '{0}'", typeof(CoverageAddedEvent).Name));
            namespaceManager.CreateSubscription("policyevents", "CoverageAddedEvent", coverageAddedFilter);
        }

    }
}
