using DataModels;
using DataModels.Exceptions;
using DataModels.Extensions;
using DataModels.Requests;
using Questly.Repositories;

namespace Questly.Services;

public class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;

    public CityService(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<List<City>> GetCitiesList()
    {
        return await _cityRepository.GetCitiesList();
    }

    public async Task<City> GetCityInfo(Guid cityId)
    {
        if(cityId == Guid.Empty)
            throw new ArgumentNullException(nameof(cityId));
        
        return await _cityRepository.GetCityInfo(cityId);
    }

    public async Task<City> CreateCityAsync(CityRequest request)
    {
        request.RequiredNotNull();

        if (await _cityRepository.DoesCityNameExistAsync(request.Name))
            throw new ConflictException($"Город «{request.Name.Trim()}» уже есть");

        return await _cityRepository.CreateCityAsync(request);
    }

    public async Task<City> UpdateCityAsync(Guid cityId, CityRequest request)
    {
        request.RequiredNotNull();

        if (cityId == Guid.Empty)
            throw new ArgumentException("INVALID_CITY_ID_PROBLEM", nameof(cityId));

        // Сам обновляемый город исключаем из проверки: иначе сохранение
        // без переименования упиралось бы в собственное же название.
        if (await _cityRepository.DoesCityNameExistAsync(request.Name, cityId))
            throw new ConflictException($"Город «{request.Name.Trim()}» уже есть");

        return await _cityRepository.UpdateCityAsync(cityId, request);
    }
}
