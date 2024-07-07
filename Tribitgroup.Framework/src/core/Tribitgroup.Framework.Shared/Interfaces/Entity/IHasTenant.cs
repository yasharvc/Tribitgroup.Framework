namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface IHasTenant
    {
        string TenantShortKey { get; }
        Task SetTenantShortKeyAsync(string tenantShortKey);
    }
}