using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernmentInformation.ViewModels
{
    public class Legislator
    {
        public static List<Legislator> retrievedLegislators = new List<Legislator> { };

        public string BioguideId { get; set; }
        public JToken JsonResponse { get; set; }
        public JToken JsonResponseCommittees { get; set; }
        public JToken JsonResponseSponsoredBills { get; set; }
        public DateTime RetrievalTime { get; set; }
        public Legislator(string bioguideId, JToken jsonResponse, JToken jsonResponeCommittees, JToken jsonResponseSponsoredBills)
        {
            BioguideId = bioguideId;
            JsonResponse = jsonResponse;
            JsonResponseCommittees = jsonResponeCommittees;
            JsonResponseSponsoredBills = jsonResponseSponsoredBills;
            RetrievalTime = DateTime.Now;
        }
    }
}
