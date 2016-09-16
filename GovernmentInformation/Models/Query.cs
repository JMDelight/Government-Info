using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace GovernmentInformation.Models
{
    public class Query
    {
        [Key]
        public int QueryId { get; set; }
        public string Description { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
