namespace TarasZoukClasses.Data.Models
{
    using Base;

    public class UserPayment : Document
    {
        public ZoukUser UserId { get; set; }

        public Payment PaymentId { get; set; }
    }
}
