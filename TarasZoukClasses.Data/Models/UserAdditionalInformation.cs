namespace TarasZoukClasses.Data.Models
{
    using Base;

    public class UserAdditionalInformation : Document
    {
        public Culture Culture { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string TelephoneNumber { get; set; }
    }
}
