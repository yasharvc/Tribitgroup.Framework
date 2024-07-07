
namespace Test.API.Authentication.Contracts
{
    public class Role : IRole
    {
        public string Name { get; set;} = string.Empty;

        public Guid Id { get; set; }
    }
}
