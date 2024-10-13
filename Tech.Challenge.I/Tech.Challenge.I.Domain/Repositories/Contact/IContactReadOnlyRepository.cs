namespace Tech.Challenge.I.Domain.Repositories.Contact;
public interface IContactReadOnlyRepository
{
    Task<bool> ThereIsRegisteredContact(Guid dddId, string phoneNumber);
    Task<IEnumerable<Entities.Contact>> RecoverAllAsync(int pageNumber, int pageSize);
    Task<IEnumerable<Entities.Contact>> RecoverByDDDIdAsync(Guid id, int pageNumber, int pageSize);
    Task<IEnumerable<Entities.Contact>> RecoverAllByDDDIdAsync(IEnumerable<Guid> dddIds, int pageNumber, int pageSize);
    Task<Entities.Contact> RecoverByContactIdAsync(Guid id);
}
