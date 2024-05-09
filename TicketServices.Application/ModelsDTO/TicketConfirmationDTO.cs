using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketServices.Application.ModelsDTO
{
    public class TicketConfirmationDTO
    {
        public Guid TicketId { get; set; }
        public string Token { get; set; }
    }

}
