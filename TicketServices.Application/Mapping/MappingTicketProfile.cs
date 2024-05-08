using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicketServices.Application.ModelsDTO;
using TicketServices.Domain.Models;

namespace TicketServices.Application.Mapping
{
    public class MappingTicketProfile : Profile
    {
        public MappingTicketProfile()
        {
            CreateMap<Ticket, TicketDTO>().ReverseMap();
        }
    }

}
