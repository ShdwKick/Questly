using DataModels;
using DataModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Questly.Authorization;
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
        if(cityId == Guid.Empty)
            return BadRequest("Invalid city ID");
        
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

    /// <summary>
    /// Создать город. Только для администраторов
    /// </summary>
    /// <response code="201">Город создан</response>
    /// <response code="400">Некорректные данные запроса</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="409">Город с таким названием уже есть</response>
    [Authorize(Policy = Policies.Admin)]
    [HttpPost]
    [ProducesResponseType(typeof(City), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<City>> CreateCity([FromBody] CityRequest request)
    {
        var city = await cityService.CreateCityAsync(request);
        return CreatedAtAction(nameof(GetCityInfo), new { cityId = city.Id }, city);
    }

    /// <summary>
    /// Обновить город. Только для администраторов
    /// </summary>
    /// <param name="cityId">Уникальный идентификатор города</param>
    /// <param name="request">Новые данные города. Передаются целиком, незаполненные поля очищаются</param>
    /// <response code="200">Город обновлён</response>
    /// <response code="400">Некорректные данные запроса</response>
    /// <response code="403">Недостаточно прав</response>
    /// <response code="404">Город не найден</response>
    /// <response code="409">Город с таким названием уже есть</response>
    [Authorize(Policy = Policies.Admin)]
    [HttpPut("{cityId:guid}")]
    [ProducesResponseType(typeof(City), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<City>> UpdateCity(Guid cityId, [FromBody] CityRequest request)
    {
        var city = await cityService.UpdateCityAsync(cityId, request);
        return Ok(city);
    }
}
