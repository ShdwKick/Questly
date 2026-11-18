using DataModels;
using DataModels.Requests;

namespace Questly.Repositories;

public interface ICityRepository
{
    Task<bool> DoesCityExist(string name);

    Task<bool> DoesCityExist(Guid cityId);

    /// <summary>
    /// Проверяет, занято ли название города, без учёта регистра.
    /// <paramref name="excludeCityId"/> исключает из проверки сам обновляемый город.
    /// </summary>
    Task<bool> DoesCityNameExistAsync(string name, Guid? excludeCityId = null);

    Task<List<City>> GetCitiesList();
    Task<City> GetCityInfo(Guid cityId);

    /// <summary>
    /// Создаёт город и возвращает сохранённую запись с присвоенным идентификатором.
    /// </summary>
    Task<City> CreateCityAsync(CityRequest request);

    /// <summary>
    /// Обновляет город целиком.
    /// </summary>
    /// <exception cref="KeyNotFoundException">Города с таким идентификатором нет.</exception>
    Task<City> UpdateCityAsync(Guid cityId, CityRequest request);
}
