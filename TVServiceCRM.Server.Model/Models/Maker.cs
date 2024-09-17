using System.Collections.ObjectModel;

namespace TVServiceCRM.Server.Model.Models
{
    public class Maker: BaseModel
    {
        public string Title { get; set; } = "";

        public virtual ICollection<MakerModel> Models { get; set; } = new Collection<MakerModel>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new Collection<Ticket>();

    }
}
