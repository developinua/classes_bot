namespace TarasZoukClasses.Data.Models
{
    using MongoDb;

    public class User : Document
    {
        public UserAdditionalInformation UserAdditionalInformationId { get; set; }

        public string NickName { get; set; }
    }
}
