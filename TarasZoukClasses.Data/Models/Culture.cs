namespace TarasZoukClasses.Data.Models
{
    using MongoDb;

    public class Culture : Document
    {
        public string Name { get; set; }

        public string LanguageCode { get; set; }
    }
}
