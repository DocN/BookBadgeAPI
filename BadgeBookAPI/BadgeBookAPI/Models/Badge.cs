using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.Models
{
    public class Badge
    {
        public string BadgeID { get; set; }
        public string ImageURL { get; set; }
        public string BadgeName { get; set; }
        public string BadgeDescription { get; set; }
    }
}
