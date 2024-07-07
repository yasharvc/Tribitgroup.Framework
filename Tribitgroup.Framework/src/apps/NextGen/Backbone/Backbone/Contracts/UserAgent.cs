namespace NextGen.Backbone.Backbone.Contracts
{
    public class UserAgent
    {
        public string IP { get; set; } = "";
        public string ClientAppVersion { get; set; } = "";
        public TimeOnly TimeZone { get; set; } = new TimeOnly(0);
        public OSTypeEnum OSType { get; set; } = OSTypeEnum.Unknown;
    }
}
