using Test.API.Authentication.Contracts;

namespace Test.API.Authentication.Providers.InMemory
{
    public class RoleProvider<Role> : IRoleProvider<Role> where Role : IRole
    {
        static List<Role> Roles = new();
        public Task DeleteAsync(Guid id)
        {
            Roles.RemoveAll(r => r.Id == id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Role>> GetAllAsync() => Task.FromResult(Roles.AsEnumerable());

        public Task<Role?> GetByIdAsync(Guid id) => Task.FromResult(Roles.FirstOrDefault(r => r.Id == id));

        public Task<IEnumerable<Role>> GetRolesByNamesAsync(params string[] roleNames) => Task.FromResult(Roles.Where(r => roleNames.Contains(r.Name)));

        public Task InsertAsync(Role role)
        {
            Roles.Add(role);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Role role)
        {
            Roles.RemoveAll(r => r.Id == role.Id);
            Roles.Add(role);
            return Task.CompletedTask;
        }
    }
}