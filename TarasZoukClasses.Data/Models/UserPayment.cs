namespace TarasZoukClasses.Data.Models
{
    using MongoDb;

    public class UserPayment : Document
    {
        public User UserId { get; set; }

        public Payment PaymentId { get; set; }
    }
}
