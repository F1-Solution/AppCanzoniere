using AppCanzoniere.Domain.Entities;

namespace AppCanzoniere.Domain.Interfaces;

public interface ILiturgicalMomentRepository
{
    Task<LiturgicalMoment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<LiturgicalMoment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<LiturgicalMoment> AddAsync(LiturgicalMoment moment, CancellationToken cancellationToken = default);
    Task UpdateAsync(LiturgicalMoment moment, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
