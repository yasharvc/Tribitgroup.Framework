namespace NextGen.Backbone.Backbone.Contracts
{
    public interface ICurrentUser<T> where T : class, IApplicationUser
    {
        T CurrentUser { get; }

    }
}
