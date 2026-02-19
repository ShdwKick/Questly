using DataModels;
using DataModels.DTOs;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class CitiesMutations
{
    private readonly ICityService _cityService;
    
    public CitiesMutations(ICityService cityService)
    {
        _cityService = cityService;
    }

    public Task<bool> CreateCity(CityDTO city)
    {
        return _cityService.CreateCity(city);
    }
    
    public Task<bool> UpdateCity(City city)
    {
        return _cityService.UpdateCities(city);
    }
    
    public Task<bool> RemoveCities(List<Guid> cities)
    {
        return _cityService.RemoveCities(cities);
    }
}