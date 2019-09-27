namespace Listrr.Data
{
    public class LanguageCode
    {

        public string Name { get; set; }

        public string Code { get; set; }

        public string Description => $"{Code} ({Name})";

    }
}