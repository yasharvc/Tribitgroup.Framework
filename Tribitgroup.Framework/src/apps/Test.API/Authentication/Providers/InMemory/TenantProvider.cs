using Test.API.Authentication.Contracts;

namespace Test.API.Authentication.Providers.InMemory
{
    public class TenantProvider<Tenant> : ITenantProvider<Tenant>
        where Tenant : ITenant
    {
        static List<Tenant> Tenants = new();
        INextShortcutGenerator ShortcutGenerator { get; }
        public TenantProvider(INextShortcutGenerator shortcutGenerator)
        {
            ShortcutGenerator = shortcutGenerator;
        }

        public async Task AddChildren(Guid id, params Tenant[] children)
        {
            var parent = await GetByIdAsync(id);
            if (parent == null)
                throw new ArgumentException("Parent not found", nameof(id));
            foreach (var child in children)
            {
                child.Path = $"<{parent.Path}><{parent.Shortcut}>";
                child.Shortcut = await ShortcutGenerator.GetNextShortcutAsync();
            }
        }

        public async Task AddChildren(string shortcut, params Tenant[] children) => await AddChildren((await GetByShortcutAsync(shortcut))?.Id ?? throw new EntryPointNotFoundException(), children);

        public Task ChangeParent(Guid id, Guid parentId)
        {
            throw new NotImplementedException();
        }

        public Task ChangeParent(string shortcut, Guid parentId)
        {
            throw new NotImplementedException();
        }

        public Task ChangeParent(string shortcut, string parentShortcut)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(params Tenant[] tenants)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tenant>> GetAllAsync() => Task.FromResult(Tenants.AsEnumerable());

        public Task<Tenant?> GetByIdAsync(Guid id) => Task.FromResult(Tenants.FirstOrDefault(t => t.Id == id));

        public Task<Tenant?> GetByShortcutAsync(string shortcut) => Task.FromResult(Tenants.FirstOrDefault(t => t.Shortcut == shortcut));

        public Task<IEnumerable<Tenant>> GetChildrenAsync(params Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Tenant>> GetChildrenAsync(params string[] shortcuts)
        {
            throw new NotImplementedException();
        }

        public Task InsertAsync(params Tenant[] tenants)
        {
            Tenants.AddRange(tenants);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(params Tenant[] tenants)
        {
            Tenants.RemoveAll(t => tenants.Any(tenant => tenant.Id == t.Id));
            Tenants.AddRange(tenants);
            return Task.CompletedTask;
        }
    }
}
