namespace Test.API.Authentication.Contracts
{
    public interface ITenant
    {
        Guid Id { get; set; }
        string Name { get; set; }
        string Shortcut { get; set; }
        string Path { get; set; }
    }
}