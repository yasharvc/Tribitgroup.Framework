namespace NextGen.Backbone.Backbone.Contracts
{
    public enum ApplicationModeEnum : int
    {
        Running,
        Starting,
        Maintenance,
        Migration,
        Stopping,
    }
}