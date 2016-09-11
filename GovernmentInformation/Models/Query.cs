using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GovernmentInformation.Models
{
    public class Query
    {
    [Key]
    public int QueryId { get; set; }
    public string Description { get; set; }
    public int UserId { get; set; }
    public virtual User User { get; set; }
  }
}
