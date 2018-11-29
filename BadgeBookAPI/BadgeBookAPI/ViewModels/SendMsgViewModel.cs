using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.ViewModels
{
    public class SendMsgViewModel
    {
        [Required]
        public string FromUID { get; set; }

        [Required]
        public string ToUID { get; set; }

        [Required] 
        public string Msg { get; set; }
    }
}
