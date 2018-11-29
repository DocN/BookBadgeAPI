using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.Models
{
    public class Message
    {
        [Key]
        public string MessageID { get; set; }
        public string Msg { get; set; }
        public string SenderUID { get; set; }
        public string ReceiverUID { get; set; }
    }
}
