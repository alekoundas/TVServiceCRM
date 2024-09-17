using System.Collections.ObjectModel;
using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.Model.Dtos
{
    public class MakerDto: BaseModel
    {
        public string? Title { get; set; }

        public virtual ICollection<MakerModel>? Models { get; set; } = new Collection<MakerModel>();
        public virtual ICollection<Ticket>? Tickets { get; set; } = new Collection<Ticket>();

    }
}
