using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TVServiceCRM.Server.Model.Dtos;
using TVServiceCRM.Server.Model.Dtos.Identity;
using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.AutoMapper
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerDto>();
            CreateMap<CustomerDto, Customer>();

            CreateMap<ContactInformation, ContactInformationDto>();
            CreateMap<ContactInformationDto, ContactInformation>();

            CreateMap<Ticket, TicketDto>();
            CreateMap<TicketDto, Ticket>();

            CreateMap<IdentityRole, IdentityRoleDto>();
            CreateMap<IdentityRoleDto, IdentityRole>();
        }
    }
}
