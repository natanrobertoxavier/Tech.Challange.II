using Tech.Challenge.I.Domain.Repositories;
using Tech.Challenge.I.Infrastructure.RepositoryAccess;

namespace Tech.Challenge.I.Integration.Tests.Fakes.Repositories;
public class WorkUnitFake(
    TechChallengeContext context) : IDisposable, IWorkUnit
{
    private readonly TechChallengeContext _context = context;
    private bool _disposed;

    public async Task Commit()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }

        _disposed = true;
    }
}