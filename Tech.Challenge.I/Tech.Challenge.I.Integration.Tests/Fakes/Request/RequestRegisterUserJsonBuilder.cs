using Tech.Challenge.I.Communication.Request;

namespace Tech.Challenge.I.Integration.Tests.Fakes.Request;
public class RequestRegisterUserJsonBuilder
{
    private string _name = "user";
    private string _email = "user@email.com";
    private string _password = "123456";

    public RequestRegisterUserJson Build()
    {
        return new RequestRegisterUserJson()
        {
            Name = _name,
            Password = _password,
            Email = _email,
        };
    }

    public RequestRegisterUserJsonBuilder WithName(string value)
    {
        _name = value;
        return this;
    }

    public RequestRegisterUserJsonBuilder WithEmail(string value)
    {
        _email = value;
        return this;
    }

    public RequestRegisterUserJsonBuilder WithPassword(string value)
    {
        _password = value;
        return this;
    }
}
