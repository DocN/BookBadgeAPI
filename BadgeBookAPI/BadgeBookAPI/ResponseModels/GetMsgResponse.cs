using BadgeBookAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.ResponseModels
{
    public class GetMsgResponse
    {
        public List<MsgContainer> MyMsgs { get; set; }
    }
}
