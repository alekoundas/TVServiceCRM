using TVServiceCRM.Server.Model.Enums;

namespace TVServiceCRM.Server.Model.Models
{
    public class Ticket: BaseModel
    {
        public TicketStatusEnum Status { get; set; }
        public TicketTypesEnum Type { get; set; }
        public string Description { get; set; } = "";   
        public string DescriptionHTML { get; set; } = "";
        public DateTime? CompletedOn { get; set; }


        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = new Customer();
    }
}
