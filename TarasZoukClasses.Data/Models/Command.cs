namespace TarasZoukClasses.Data.Models
{
    using Base;

    public class Command : Document
    {
        public string Name { get; set; }

        public bool IsActive { get; set; }
    }
}
