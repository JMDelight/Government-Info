using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GovernmentInformation.ViewModels
{
    public class ApiCall
    {
        public int ColumnId { get; set; }
        public string CallType { get; set; }
        public string BioguideId { get; set; }
        public string CommitteeId { get; set; }
        public string BillID { get; set; }

        public ApiCall(int columnId, string callType, string bioguideId = null, string committeeId = null, string billId = null)
        {
            ColumnId = columnId;
            CallType = callType;
            BioguideId = bioguideId;
            CommitteeId = committeeId;
            BillID = billId;
        }
    }
}
