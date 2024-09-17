using TVServiceCRM.Server.Model.Models;

namespace TVServiceCRM.Server.Model.Dtos
{
    public class MakerModelDto: BaseModel
    {
        public string? Title { get; set; }

        public int? MakerId { get; set; }
        public Maker? Maker { get; set; }
    }
}
