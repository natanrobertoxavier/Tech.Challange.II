using Tech.Challenge.I.Domain.Repositories;

namespace Tech.Challenge.I.Integration.Tests.Fakes;
public class FakeWorkUnit : IWorkUnit
{
    public Task Commit()
    {
        throw new NotImplementedException();
    }
}
