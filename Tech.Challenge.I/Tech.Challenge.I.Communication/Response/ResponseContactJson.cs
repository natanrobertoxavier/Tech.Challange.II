namespace Tech.Challenge.I.Communication.Response;
public class ResponseContactJson
{
    public Guid ContactId { get; set; }
    public string Region { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int DDD { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
}
