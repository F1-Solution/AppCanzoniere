using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Interfaces;
using AppCanzoniere.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AppCanzoniere.Infrastructure.Repositories;

public class LiturgicalMomentRepository : ILiturgicalMomentRepository
{
    private readonly AppDbContext _context;

    public LiturgicalMomentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LiturgicalMoment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LiturgicalMoments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IEnumerable<LiturgicalMoment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LiturgicalMoments
            .OrderBy(lm => lm.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<LiturgicalMoment> AddAsync(LiturgicalMoment moment, CancellationToken cancellationToken = default)
    {
        _context.LiturgicalMoments.Add(moment);
        await _context.SaveChangesAsync(cancellationToken);
        return moment;
    }

    public async Task UpdateAsync(LiturgicalMoment moment, CancellationToken cancellationToken = default)
    {
        _context.LiturgicalMoments.Update(moment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var moment = await GetByIdAsync(id, cancellationToken);
        if (moment != null)
        {
            _context.LiturgicalMoments.Remove(moment);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
