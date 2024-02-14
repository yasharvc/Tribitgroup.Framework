namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface IHasCreatedBy
    {
        Guid CreatedBy { get; }
        Task SetCreatedByAsync(Guid userId);
    }
}