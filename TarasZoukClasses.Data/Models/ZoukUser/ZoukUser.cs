namespace TarasZoukClasses.Data.Models.ZoukUser
{
    using Base;

    public class ZoukUser : Document
    {
        public ZoukUserAdditionalInformation ZoukUserAdditionalInformation { get; set; }

        public string NickName { get; set; }
    }
}
