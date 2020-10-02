﻿namespace TarasZoukClasses.Data.Models
{
    using MongoDb;

    public class Command : Document
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
