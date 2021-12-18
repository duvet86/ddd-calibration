using Ardalis.Specification;

namespace ddd.SharedKernel.Interfaces
{
    public interface IReadRepository<T> : IReadRepositoryBase<T> where T : class, IAggregateRoot
    {
    }
}