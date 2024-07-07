namespace Test.API.Authentication.Contracts
{
    public interface ITenantProvider<Tenant> where Tenant : ITenant
    {
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant?> GetByIdAsync(Guid id);
        Task<Tenant?> GetByShortcutAsync(string shortcut);
        Task<IEnumerable<Tenant>> GetChildrenAsync(params Guid[] ids);
        Task<IEnumerable<Tenant>> GetChildrenAsync(params string[] shortcuts);

        Task InsertAsync(params Tenant[] tenants);
        Task UpdateAsync(params Tenant[] tenants);
        Task DeleteAsync(params Tenant[] tenants);
        Task AddChildren(Guid id, params Tenant[] children);
        Task AddChildren(string shortcut, params Tenant[] children);
        Task ChangeParent(Guid id, Guid parentId);
        Task ChangeParent(string shortcut, Guid parentId);
        Task ChangeParent(string shortcut, string parentShortcut);
    }
}
