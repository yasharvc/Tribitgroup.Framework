namespace Test.API.Authentication.Contracts
{
    public interface INextShortcutGenerator
    {
        Task<string> GetNextShortcutAsync();
    }
}