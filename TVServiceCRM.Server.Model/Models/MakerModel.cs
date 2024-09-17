using TVServiceCRM.Server.Model.Enums;

namespace TVServiceCRM.Server.Model.Models
{
    public class MakerModel : BaseModel
    {
        public string Title { get; set; } = "";

        public int MakerId { get; set; }
        public Maker Maker { get; set; } = new Maker();
    }
}
