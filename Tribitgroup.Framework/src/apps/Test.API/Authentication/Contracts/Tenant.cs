namespace Test.API.Authentication.Contracts
{
    public class Tenant : ITenant
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; } = string.Empty;
        public string Shortcut { get; set; } = string.Empty; 
        public string Path { get; set; } = string.Empty;
    }
}