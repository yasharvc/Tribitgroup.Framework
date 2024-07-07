using NextGen.Backbone.Backbone.Contracts;

namespace NextGen
{
    public class ApplicationUser : IApplicationUser
    {
        public string Name { get; set; } = $"Yashar {DateTime.Now.Millisecond}";

        public void SetName(string name)
        {
            Name = name;
        }
    }
}