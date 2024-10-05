using Tech.Challenge.I.Communication.Request;

namespace Tech.Challenge.I.Integration.Tests.Fakes.Request;
public class RequestChangePasswordJsonBuilder
{
    private string _currentPassword = "123456";
    private string _newPassword = "123456";

    public RequestChangePasswordJson Build()
    {
        return new RequestChangePasswordJson()
        {
            CurrentPassword = _currentPassword,
            NewPassword = _newPassword
        };
    }

    public RequestChangePasswordJsonBuilder WithCurrentPassword(string value)
    {
        _currentPassword = value;
        return this;
    }

    public RequestChangePasswordJsonBuilder WithNewPassword(string value)
    {
        _newPassword = value;
        return this;
    }
}