using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.Model.Dtos
{
    public class TicketDto: BaseModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? CompletedOn { get; set; }


        public int? CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
    }
}
