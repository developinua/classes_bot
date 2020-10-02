namespace TarasZoukClasses.Data.Models
{
    using MongoDb;

    public class UserAdditionalInformation : Document
    {
        public Culture CultureId { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public string TelephoneNumber { get; set; }
    }
}
