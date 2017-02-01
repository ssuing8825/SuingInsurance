using System.Collections.Generic;

namespace SuingInsurance.Domain.InputCommands
{
    public class CreatePolicyCommand
    {
        public List<string> Coverage { get; set; }

        public string PolicyNumber { get; set; }
    }

    public class AddCoverageCommand
    {
        public string Coverage { get; set; }
    }
}
