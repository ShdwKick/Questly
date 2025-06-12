using DataModels;
using DataModels.DTOs;

namespace QuestlyAdmin.Services
{
    public interface ICityService
    {
        Task<List<City>> GetCitiesList();
        Task<City> GetCityInfo(Guid cityId);
        Task<bool> CreateCities(List<CityDTO> cities);
        Task<bool> UpdateCities(City city);
        Task<bool> RemoveCities(List<Guid> cities);
    }
}