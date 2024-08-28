using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.Model.Dtos
{
    public class CustomerDto : BaseModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; } 

        public virtual IEnumerable<TicketDto>? Tickets { get; set; } 
        public virtual IEnumerable<ContactInformationDto>? ContactInformations { get; set; } 

    }
}
