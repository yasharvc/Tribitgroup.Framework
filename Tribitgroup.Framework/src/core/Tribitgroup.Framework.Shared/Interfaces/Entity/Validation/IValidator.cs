namespace Tribitgroup.Framework.Shared.Interfaces.Entity.Validation
{
    public interface IValidator { }
    public interface IValidator<T> : IValidator
    {
        Task ValidateAsync(T input);
    }
}