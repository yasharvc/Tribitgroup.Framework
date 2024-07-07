namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface IHasUpdatedBy
    {
        Guid UpdatedBy { get; }
        Task SetUpdatedByAsync(DateTime updatedAt);
    }
}