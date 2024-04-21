using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserServices.Domain.Models
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string RefeshToken { get; set; } 
        public Guid UserId { get; set; } 
        public DateTime ExpirationTime { get; set; } 
    }
}
