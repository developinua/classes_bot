﻿namespace TarasZoukClasses.Data.Models
{
    using Base;

    public class ZoukUserSubscription : Document
    {
        public ZoukUser.ZoukUser ZoukUser { get; set; }

        public Subscription.Subscription Subscription { get; set; }

        public int RemainingClassesCount { get; set; }
    }
}