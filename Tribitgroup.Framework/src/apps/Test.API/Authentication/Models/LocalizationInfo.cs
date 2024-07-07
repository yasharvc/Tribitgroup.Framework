using Test.API.Authentication.Enums;

namespace Test.API.Authentication.Models
{
    public class LocalizationInfo
    {
        public LanguageCodeIdentifierEnum Language { get; set; }
        public TimeZone TimeZone { get; set; } = new TimeZone();
    }
}
