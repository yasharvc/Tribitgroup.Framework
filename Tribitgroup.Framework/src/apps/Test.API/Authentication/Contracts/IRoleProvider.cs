namespace Test.API.Authentication.Contracts
{
    public interface IRoleProvider<Role> where Role : IRole
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task InsertAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Guid id);
        Task<Role?> GetByIdAsync(Guid id);
        Task<IEnumerable<Role>> GetRolesByNamesAsync(params string[] roleNames);
    }
}