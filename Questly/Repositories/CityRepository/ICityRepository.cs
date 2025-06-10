using DataModels;

namespace Questly.Repositories
{
    public interface ICityRepository
    {
        Task<bool> DoesCityExist(string name);
    
        Task<bool> DoesCityExist(Guid cityId);
    
        Task<List<City>> GetCitiesList();
        Task<City> GetCityInfo(Guid cityId);
    }
}