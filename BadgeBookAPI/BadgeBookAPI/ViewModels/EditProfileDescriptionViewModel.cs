using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.ViewModels
{
    public class EditProfileDescriptionViewModel
    {
        [Required]
        public string Description { get; set; }
    }
}
