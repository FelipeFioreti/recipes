using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Recipes.Api.Domain.DTOs.Recipes;
using Recipes.Api.Domain.Entities.Enums;
using Recipes.Api.Domain.Exceptions;
using Recipes.Api.Domain.Interfaces.Recipes;

namespace Recipes.Api.Presentation.Controllers.Recipes;

[ApiController]
[Route("api/[controller]")]
public class UnitController(IUnitService unitService) : ControllerBase
{
    [Authorize(Roles = nameof(Roles.USER))]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UnitResponse>> Get(int id)
    {
        var unit = await unitService.GetById(id);

        return unit == null
            ? throw new NotFoundException($"Unit with id '{id}' was not found.")
            : Ok(unit);
    }

    [Authorize(Roles = nameof(Roles.USER))]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitResponse>>> GetAll([FromQuery] int page = 0,
        [FromQuery] int size = 10)
    {
        return Ok(await unitService.GetAll(page, size));
    }

    [Authorize(Roles = nameof(Roles.ADMIN))]
    [HttpPost]
    public async Task<ActionResult<UnitResponse>> Create([FromBody] CreateUnitRequest request)
    {
        var unit = await unitService.Create(request);

        return unit == null
            ? throw new BadRequestException("Failed to create unit.")
            : Ok(unit);
    }

    [Authorize(Roles = nameof(Roles.ADMIN))]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UnitResponse>> Update(int id, [FromBody] UpdateUnitRequest request)
    {
        var unit = await unitService.Update(id, request);

        return unit == null
            ? throw new NotFoundException($"Unit with id '{id}' was not found.")
            : Ok(unit);
    }

    [Authorize(Roles = nameof(Roles.ADMIN))]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        var disabled = await unitService.Disable(id);

        return !disabled ? throw new NotFoundException($"Unit with id '{id}' was not found.") : NoContent();
    }
}
