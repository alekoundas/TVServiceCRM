using System;
namespace TVServiceCRM.Server.Model.Models
{
    public class Ticket: BaseModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CompletedOn { get; set; }


        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
