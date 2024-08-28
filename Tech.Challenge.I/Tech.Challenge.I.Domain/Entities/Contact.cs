namespace Tech.Challenge.I.Domain.Entities;
public class Contact : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Guid DDDId { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public Guid UserId { get; set; }
}
