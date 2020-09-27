using MongoDB.Driver;

namespace TarasZoukClasses.Data.Models
{
    public class MongoDbSettings
    {
        public string Connection { get; set; }

        public string DatabaseName { get; set; }
    }
}
