using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GovernmentInformation.Models
{
    public class User
    {
    [Key]
    public int UserId { get; set; }
    public string UserName { get; set; }
    public virtual ICollection<Query> Queries { get; set; }

  }
}
