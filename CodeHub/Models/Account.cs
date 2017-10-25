using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeHub.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string AvatarUrl { get; set; }
        public bool IsLoggedIn { get; set; }
    }
}
