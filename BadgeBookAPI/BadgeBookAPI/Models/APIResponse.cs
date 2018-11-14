using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.Models
{
    public class APIResponse
    {
        public Object Data { get; set; }
        public string Message { get; set;  }
        public bool Success { get; set; }
        public APIResponse()
        {
            this.Message = "";
            this.Success = true;
        }
    }
}
