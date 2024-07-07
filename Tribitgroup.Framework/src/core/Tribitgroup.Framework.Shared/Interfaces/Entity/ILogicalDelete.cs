namespace Tribitgroup.Framework.Shared.Interfaces.Entity
{
    public interface ILogicalDelete
    {
        bool Deleted { get; }
        Task DeleteAsync();
        Task UndeleteAsync();
    }
}