using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SuingInsurance.Domain;
using SuingInsurance.Domain.Aggregates;
using SuingInsurance.Domain.InputCommands;

namespace SuingInsurance.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void CreatePolicy_WithValidData_ShouldSucceed()
        {
            var policyNumber = Guid.NewGuid().ToString();
            var serviceLocator = new ServiceLocatorProduction();

            //New up the domain model. Everything is null, but it's ready to work
            var insurancePolicy = new InsurancePolicy(serviceLocator);

            //Create the policy. This is the only way you can create a policy
            insurancePolicy.CreatePolicy(new CreatePolicyCommand() { Coverage = new List<string>() { "CoverageA" }, PolicyNumber = policyNumber });

            //Verify the domain model has been updated
            Assert.AreEqual(1, insurancePolicy.Coverage.Count, string.Format("Should only be 1 coverage but there were {0}", insurancePolicy.Coverage.Count));

            //Reset the system so we can verify getting the object from the stream
            insurancePolicy = new InsurancePolicy(serviceLocator);
            insurancePolicy.GetPolicyByPolicyNumber(policyNumber);
            Assert.AreEqual(1, insurancePolicy.Coverage.Count, string.Format("Should only be 1 coverage but there were {0}", insurancePolicy.Coverage.Count));

            //Add a coverage to the policy
            insurancePolicy.AddCoverageToPolicy(new AddCoverageCommand() { Coverage = "CoverageB" });
            Assert.AreEqual(2, insurancePolicy.Coverage.Count, string.Format("Should 2 coverage but there were {0}", insurancePolicy.Coverage.Count));

            //Verify that we actually added the coverage
            Assert.AreEqual(2, insurancePolicy.Coverage.Count, string.Format("Should 2 coverage but there were {0}", insurancePolicy.Coverage.Count));

            //Reset the system so we can verify the coverage was added. 
            insurancePolicy = new InsurancePolicy(serviceLocator);
            insurancePolicy.GetPolicyByPolicyNumber(policyNumber);
            Assert.AreEqual(2, insurancePolicy.Coverage.Count, string.Format("Should 2 coverage but there were {0}", insurancePolicy.Coverage.Count));

        }

        [TestMethod]
        public void PolicyIdParsingTest()
        {
            var serviceLocator = new ServiceLocatorProduction();


            var policy = new InsurancePolicy(serviceLocator);
            policy.Id = "ABSC_122";

            Assert.AreEqual("ABSC", policy.PolicyNumber);
            Assert.AreEqual(122, policy.Version);
        }
    }
}
