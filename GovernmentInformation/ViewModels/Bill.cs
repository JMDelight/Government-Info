using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernmentInformation.ViewModels
{
    public class Bill
    {
        public static List<Bill> retrievedBills = new List<Bill> { };

        public string BillId { get; set; }
        public JToken JsonResponse { get; set; }
        public string BillText { get; set; }
        public DateTime RetrievalTime { get; set; }
        public Bill(string billId, JToken jsonResponse, string billText)
        {
            JsonResponse = jsonResponse;
            BillId = billId;
            BillText = billText;
            RetrievalTime = DateTime.Now;
        }
    }
}
