using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Application.DTOs.Recipes;
using Recipes.Application.Interfaces.Recipes;
using Recipes.Domain.Entities.Enums;
using Recipes.Domain.Exceptions;

namespace Recipes.Api.Controllers.Recipes;

[ApiController]
[Authorize(Roles = nameof(Roles.USER))]
[Route("api/[controller]")]
public class StepController(IStepService stepService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<StepResponse>> GetById(int id)
    {
        var step = await stepService.GetById(id);

        return step == null
            ? throw new NotFoundException($"Step with id '{id}' was not found.")
            : Ok(step);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StepResponse>>> GetAll([FromQuery] int page = 0,
        [FromQuery] int size = 10)
    {
        return Ok(await stepService.GetAll(page, size));
    }

    [HttpPost]
    public async Task<ActionResult<StepResponse>> Create([FromBody] CreateStepRequest request)
    {
        var step = await stepService.Create(request);

        return step == null
            ? throw new BadRequestException("Failed to create step.")
            : Ok(step);
    }

    [HttpPut("")]
    public async Task<ActionResult<StepResponse>> Update([FromBody] UpdateStepRequest request)
    {
        var step = await stepService.Update(request);

        return step == null
            ? throw new NotFoundException($"Step with id '{request.Id}' was not found.")
            : Ok(step);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        var disabled = await stepService.Disable(id);

        if (!disabled)
            throw new NotFoundException($"Step with id '{id}' was not found.");

        return NoContent();
    }
}
