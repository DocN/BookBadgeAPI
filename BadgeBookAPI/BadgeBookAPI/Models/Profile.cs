using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.Models
{
    public class Profile
    {
        [Key]
        public string ProfileID { get; set; }
        public string UID { get; set; }
        public string Description { get; set; }
    }
}
