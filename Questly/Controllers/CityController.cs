using DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questly.Services;

namespace Questly.Controllers;

[Authorize]
[ApiController]
[Route("api/cities")]
public class CitiesController(ICityService cityService) : ControllerBase
{
    /// <summary>
    /// Получить данные о конкретном городе
    /// </summary>
    /// <param name="cityId">Уникальный идентификатор города</param>
    /// <response code="200">Город успешно найден</response>
    /// <response code="404">Город не найден</response>
    [HttpGet("{cityId:guid}")]
    [ProducesResponseType(typeof(City), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<City>> GetCityInfo(Guid cityId)
    {
        var city = await cityService.GetCityInfo(cityId);
        return city is null ? NotFound() : Ok(city);
    }

    /// <summary>
    /// Получить список всех городов
    /// </summary>
    /// <response code="200">Список городов успешно возвращен</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<City>), 200)]
    public async Task<ActionResult<List<City>>> GetCitiesList()
    {
        var cities = await cityService.GetCitiesList();
        return Ok(cities);
    }
}