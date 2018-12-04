using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.Models
{
    public class Badge
    {
        [Key]
        public string BadgeID { get; set; }
        public string UID { get; set; }
        public string ImageURL { get; set; }
        public string BadgeName { get; set; }
        public string BadgeDescription { get; set; }

        public string ApplicationId { get; set; }
    }
}
