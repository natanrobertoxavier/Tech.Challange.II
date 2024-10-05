using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Tech.Challenge.I.Api.Filters;
using Tech.Challenge.I.Application.UseCase.DDD.Recover;
using Tech.Challenge.I.Application.UseCase.DDD.Register;
using Tech.Challenge.I.Communication.Request;
using Tech.Challenge.I.Communication.Request.Enum;
using Tech.Challenge.I.Communication.Response;

namespace Tech.Challenge.I.Api.Controllers.v1;

[ServiceFilter(typeof(AuthenticatedUserAttribute))]
public class RegionDDDController : TechChallengeController
{
    [HttpPost]
    [ProducesResponseType(typeof(IEnumerable<ResponseRegionDDDJson>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDDD(
        [FromServices] IRegisterRegionDDDUseCase useCase,
        [FromBody] RequestRegionDDDJson request)
    {
        await useCase.Execute(request);

        return Created(nameof(RecoverAll), null);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ResponseRegionDDDJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RecoverAll(
        [FromServices] IRecoverRegionDDDUseCase useCase)
    {
        var result = await useCase.Execute();

        if (result.Any())
            return Ok(result);

        return NoContent();
    }

    [HttpGet]
    [Route("DDD/by-region")]
    [ProducesResponseType(typeof(IEnumerable<ResponseRegionDDDJson>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RecoverByRegion(
        [FromQuery][Required] RegionRequestEnum region,
        [FromServices] IRecoverRegionDDDUseCase useCase)
    {
        var result = await useCase.Execute(region);

        if (result.Any())
            return Ok(result);

        return NoContent();
    }
}
