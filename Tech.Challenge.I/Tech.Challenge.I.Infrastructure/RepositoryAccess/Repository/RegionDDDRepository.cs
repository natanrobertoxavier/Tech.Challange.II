using Microsoft.EntityFrameworkCore;
using Tech.Challenge.I.Domain.Entities;
using Tech.Challenge.I.Domain.Repositories.DDD;

namespace Tech.Challenge.I.Infrastructure.RepositoryAccess.Repository;
public class RegionDDDRepository(
    TechChallengeContext context) : IRegionDDDReadOnlyRepository, IRegionDDDWriteOnlyRepository
{
    private readonly TechChallengeContext _context = context;

#pragma warning disable CS8603 // Possível retorno de referência nula.
    public async Task Add(RegionDDD ddd) =>
        await _context.DDDRegions.AddAsync(ddd);

    public async Task<IEnumerable<RegionDDD>> RecoverAllAsync()
        => await _context.DDDRegions.AsNoTracking().ToListAsync();

    public async Task<IEnumerable<RegionDDD>> RecoverListByDDDAsync(int dDD) =>
        await _context.DDDRegions.Where(c => c.DDD.Equals(dDD)).ToListAsync();

    public async Task<IEnumerable<RegionDDD>> RecoverListDDDByRegionAsync(string region) =>
        await _context.DDDRegions.Where(r => r.Region == region).ToListAsync();

    public async Task<bool> ThereIsDDDNumber(int ddd) =>
        await _context.DDDRegions.AnyAsync(c => c.DDD.Equals(ddd));

    public async Task<RegionDDD> RecoverByIdAsync(Guid id) =>
        await _context.DDDRegions.Where(c => c.Id.Equals(id)).FirstOrDefaultAsync();

    public async Task<RegionDDD> RecoverByDDDAsync(int dDD) =>
        await _context.DDDRegions.Where(c => c.DDD.Equals(dDD)).FirstOrDefaultAsync();
#pragma warning restore CS8603 // Possível retorno de referência nula.
}
