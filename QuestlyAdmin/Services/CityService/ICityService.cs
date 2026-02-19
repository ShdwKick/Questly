using DataModels;
using DataModels.DTOs;

namespace QuestlyAdmin.Services
{
    public interface ICityService
    {
        Task<List<City>> GetCitiesList();
        Task<City> GetCityInfo(Guid cityId);
        Task<bool> CreateCity(CityDTO city);
        Task<bool> UpdateCities(City city);
        Task<bool> RemoveCities(List<Guid> cities);
    }
}