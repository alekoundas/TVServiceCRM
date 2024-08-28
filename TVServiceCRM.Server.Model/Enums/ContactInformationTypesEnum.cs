using System.Runtime.Serialization;

namespace TVServiceCRM.Server.Model.Enums
{
    [DataContract]
    [Serializable]

    public enum ContactInformationTypesEnum
    {
        [EnumMember(Value = "EMAIL")]
        EMAIL,
        [EnumMember(Value = "PHONE")]
        PHONE,
    }
}
