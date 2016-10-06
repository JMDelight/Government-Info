using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernmentInformation.ViewModels
{

    public class Committee
    {
        public static List<Committee> retrievedCommittees = new List<Committee> { };

        public string CommitteeId { get; set; }
        public JToken JsonResponse { get; set; }
        public string ParentCommitteeId { get; set; }
        public bool IsSubCommittee { get; set; }
        public JToken SubCommittees { get; set; }
        public DateTime RetrievalTime { get; set; }
        public JToken CommitteeMembers { get; set; }
        public JToken CommitteesBills { get; set; }
        public Committee(string committeeId, JToken jsonResponse, JToken committeeMembers, bool isSubCommittee, JToken subCommittees = null, string parentCommitteeId = null, JToken committeesBills = null)
        {
            JsonResponse = jsonResponse;
            CommitteeId = committeeId;
            RetrievalTime = DateTime.Now;
            IsSubCommittee = isSubCommittee;
            SubCommittees = subCommittees;
            ParentCommitteeId = parentCommitteeId;
            CommitteesBills = committeesBills;
            CommitteeMembers = committeeMembers;
        }
    }
}
