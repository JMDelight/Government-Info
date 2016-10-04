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
        public JObject JsonRespone { get; set; }
        public string CommitteeText { get; set; }
        public DateTime RetrievalTime { get; set; }
        public Committee(string committeeId, JObject jsonResponse)
        {
            JsonRespone = jsonResponse;
            CommitteeId = committeeId;
            RetrievalTime = DateTime.Now;
        }
    }
}
