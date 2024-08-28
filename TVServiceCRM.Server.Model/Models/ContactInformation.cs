using TVServiceCRM.Server.Model.Enums;

namespace TVServiceCRM.Server.Model.Models
{
    public class ContactInformation : BaseModel
    {

        public ContactInformationTypesEnum Type { get; set; }
        public string Value{ get; set; }
        public string Description { get; set; }
        public bool IsFavorite { get; set; }


        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
