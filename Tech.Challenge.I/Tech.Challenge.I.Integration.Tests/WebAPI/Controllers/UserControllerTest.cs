using Moq;
using Tech.Challenge.I.Application.UseCase.User.Register;
using Tech.Challenge.I.Integration.Tests.WebAPI.Controllers.Base;

namespace Tech.Challenge.I.Integration.Tests.WebAPI.Controllers;
public class UserControllerTest : BaseControllerTest
{
    private readonly Mock<IRegisterUserUseCase> _useCase;
    private string _urlBase => Path.Combine($"{ApiUrl}", "user");

    public UserControllerTest() : base()
    {
        _useCase = new Mock<IRegisterUserUseCase>();
    }

    [Fact]
    public async Task UserController_ReturnsCreated_WhenUserIsCreated()
    {
        var url = _urlBase;
        var response = await _httpClient.PostAsync(url, null);

        Assert.True(response.IsSuccessStatusCode);
    }
}
