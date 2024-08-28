namespace Tech.Challenge.I.Domain.Repositories.Contact;
public interface IContactReadOnlyRepository
{
    Task<bool> ThereIsRegisteredContact(Guid dddId, string phoneNumber);
    Task<IEnumerable<Entities.Contact>> RecoverAllAsync();
    Task<IEnumerable<Entities.Contact>> RecoverByDDDIdAsync(Guid id);
    Task<IEnumerable<Entities.Contact>> RecoverAllByDDDIdAsync(IEnumerable<Guid> dddIds);
    Task<Entities.Contact> RecoverByContactIdAsync(Guid id);
}
