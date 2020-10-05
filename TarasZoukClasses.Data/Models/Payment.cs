namespace TarasZoukClasses.Data.Models
{
    using System;
    using Base;

    public class Payment : Document
    {
        public decimal Amount { get; set; }

        public bool IsSuccess { get; set; }

        public DateTime? DateTimeProcessed { get; set; }
    }
}
