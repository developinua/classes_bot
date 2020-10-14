namespace TarasZoukClasses.Data.Models
{
    using Base;

    public class ZoukUserAdditionalInformation : Document
    {
        public Culture Culture { get; set; }

        public int ChatId { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string TelephoneNumber { get; set; }
    }
}
