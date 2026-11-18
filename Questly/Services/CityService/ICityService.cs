using DataModels;
using DataModels.Requests;

namespace Questly.Services;

public interface ICityService
{
    Task<List<City>> GetCitiesList();
    Task<City> GetCityInfo(Guid cityId);

    /// <summary>
    /// Создаёт город. Название должно быть свободно без учёта регистра.
    /// </summary>
    Task<City> CreateCityAsync(CityRequest request);

    /// <summary>
    /// Обновляет город целиком.
    /// </summary>
    Task<City> UpdateCityAsync(Guid cityId, CityRequest request);
}
