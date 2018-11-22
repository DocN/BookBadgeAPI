using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadgeBookAPI.Models
{
    public class CompactIdentityUser
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string UID { get; set; }
        public int BirthDay { get; set; }
        public int BirthMonth { get; set; }
        public int BirthYear { get; set; }
        public UserData UserData { get; set; }
    }
}
