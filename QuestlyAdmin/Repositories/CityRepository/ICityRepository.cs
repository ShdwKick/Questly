using DataModels;
using DataModels.DTOs;

namespace QuestlyAdmin.Repositories
{
    public interface ICityRepository
    {
        Task<bool> DoesCityExistAsync(string name);
    
        Task<bool> DoesCityExistAsync(Guid cityId);
    
        Task<List<City>> GetCitiesList();
        Task<City> GetCityInfo(Guid cityId);

        Task<bool> CreateCities(List<CityDTO> cities);
        Task<bool> UpdateCity(City city);
        Task<bool> RemoveCities(List<Guid> cities);
    }
}