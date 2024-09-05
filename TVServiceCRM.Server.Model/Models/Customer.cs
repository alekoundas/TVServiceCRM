using System.Collections.ObjectModel;

namespace TVServiceCRM.Server.Model.Models
{
    public class Customer : BaseModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public virtual ICollection<Ticket> Tickets { get; set; } =  new Collection<Ticket>();
        public virtual ICollection<ContactInformation> ContactInformations { get; set; } = new Collection<ContactInformation>();

    }
}
