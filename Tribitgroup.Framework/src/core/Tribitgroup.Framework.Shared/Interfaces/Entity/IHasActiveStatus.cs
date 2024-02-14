namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface IHasActiveStatus
    {
        bool IsActive { get; }
        Task ActivateAsync();
        Task DeactivateAsync();
    }
}