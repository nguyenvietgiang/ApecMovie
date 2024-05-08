using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketServices.Application.ModelsDTO
{
    public class TicketDTO
    {
        public Guid MovieID { get; set; }
        public int SeatNumber { get; set; }
        public DateTime ShowTime { get; set; }
        public Guid UserID { get; set; }
    }

}
