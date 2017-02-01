using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using SuingInsurance.Domain;
using SuingInsurance.Domain.Aggregates;
using SuingInsurance.Domain.InputCommands;

namespace SuingInsurance.Api.Webrole.Controllers
{

    public class PolicyController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        public ServiceLocatorBase ServiceLocator
        {
            get
            {
                // IMPORTANT: Don't use backing field for this.
                return ((WebApiApplication)HttpContext.Current.ApplicationInstance).ServiceLocator;
            }
        }

        //// GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/values
        public void Post([FromBody]CreatePolicyCommand command)
        {
            var insurancePolicy = new InsurancePolicy(this.ServiceLocator);

            //Create the policy. This is the only way you can create a policy
            insurancePolicy.CreatePolicy(command);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
