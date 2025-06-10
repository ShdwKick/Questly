using DataModels;

namespace Questly.Services
{
    public interface ICityService
    {
        Task<List<City>> GetCitiesList();
        Task<City> GetCityInfo(Guid cityId);
    }
}