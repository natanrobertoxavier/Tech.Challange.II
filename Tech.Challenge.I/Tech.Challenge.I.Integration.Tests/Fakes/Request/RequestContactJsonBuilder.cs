using Tech.Challenge.I.Communication.Request;

namespace Tech.Challenge.I.Integration.Tests.Fakes.Request;
public class RequestContactJsonBuilder
{
    private string _firstName = "John";
    private string _lastName = "Cena";
    private int _dDD = 11;
    private string _phoneNumber = "94400-8791";
    private string _email = "john@email.com";

    public RequestContactJson Build()
    {
        return new RequestContactJson()
        {
            FirstName = _firstName,
            LastName = _lastName,
            DDD = _dDD,
            PhoneNumber = _phoneNumber,
            Email = _email
        };
    }

    public RequestContactJsonBuilder WithFirstName(string value)
    {
        _firstName = value;
        return this;
    }

    public RequestContactJsonBuilder WithLastName(string value)
    {
        _lastName = value;
        return this;
    }

    public RequestContactJsonBuilder WithDDD(int value)
    {
        _dDD = value;
        return this;
    }

    public RequestContactJsonBuilder WithPhoneNumber(string value)
    {
        _phoneNumber = value;
        return this;
    }

    public RequestContactJsonBuilder WithEmal(string value)
    {
        _email = value;
        return this;
    }
}
