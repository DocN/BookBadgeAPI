﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.ViewModels
{
    public class EditUserProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public int BirthDay { get; set; }
        [Required]
        public int BirthMonth { get; set; }
        [Required]
        public int BirthYear { get; set; }
    }
}