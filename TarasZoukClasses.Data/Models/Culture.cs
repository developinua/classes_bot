namespace TarasZoukClasses.Data.Models
{
    using Base;

    public class Culture : Document
    {
        public string Name { get; set; }

        public string LanguageCode { get; set; }
    }
}
