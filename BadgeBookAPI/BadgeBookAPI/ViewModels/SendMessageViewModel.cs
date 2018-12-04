using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.ViewModels
{
    public class SendMessageViewModel
    {
        [Required]
        public string MsgToUID { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Msg { get; set; }
    }
}
