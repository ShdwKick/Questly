using DataModels;

namespace QuestlyAdmin.Services
{
    public interface ICityService
    {
        Task<List<City>> GetCitiesList();
        Task<City> GetCityInfo(Guid cityId);
    }
}