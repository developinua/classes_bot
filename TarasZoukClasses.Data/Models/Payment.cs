namespace TarasZoukClasses.Data.Models
{
    using Base;

    public class Payment : Document
    {
        public ZoukUserSubscription ZoukUserSubscription { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }
    }
}
