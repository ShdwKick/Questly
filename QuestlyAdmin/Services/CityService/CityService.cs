using DataModels;
using QuestlyAdmin.Repositories;

namespace QuestlyAdmin.Services
{
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
    }
}