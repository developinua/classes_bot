namespace TarasZoukClasses.Data.Models
{
    using Base;

    public class ZoukUser : Document
    {
        public UserAdditionalInformation UserAdditionalInformation { get; set; }

        public string NickName { get; set; }
    }
}
