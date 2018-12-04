using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.ViewModels
{
    public class UpdateBadgeViewModel
    {
        [Required]
        public string BadgeID { get; set; }
        [Required]
        public string ImageURL { get; set; }
        [Required]
        public string BadgeName { get; set; }
        [Required]
        public string BadgeDescription { get; set; }
    }
}
