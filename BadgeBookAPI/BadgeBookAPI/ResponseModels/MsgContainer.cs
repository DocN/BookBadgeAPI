using BadgeBookAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.ResponseModels
{
    public class MsgContainer
    {
        public Message Msg { get; set; }
        public string SenderEmail { get; set; }
    }
}
