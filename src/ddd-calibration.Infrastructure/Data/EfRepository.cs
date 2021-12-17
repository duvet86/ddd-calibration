using Ardalis.Specification.EntityFrameworkCore;
using ddd_calibration.SharedKernel.Interfaces;

namespace ddd_calibration.Infrastructure.Data;

// inherit from Ardalis.Specification type
public class EfRepository<T> : RepositoryBase<T>, IReadRepository<T>, IRepository<T> where T : class, IAggregateRoot
{
  public EfRepository(AppDbContext dbContext) : base(dbContext)
  {
  }
}
