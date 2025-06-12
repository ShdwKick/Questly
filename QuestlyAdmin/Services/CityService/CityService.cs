using DataModels;
using DataModels.DTOs;
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

        public Task<bool> CreateCities(List<CityDTO> cities)
        {
            if(cities == null || cities.Count == 0)
                throw new ArgumentNullException(nameof(cities));
            return _cityRepository.CreateCities(cities);
        }

        public async Task<bool> UpdateCities(City city)
        {
            if(city == null || city.Id == Guid.Empty)
                throw new ArgumentNullException(nameof(city));
            
            if(!await _cityRepository.DoesCityExistAsync(city.Id))
                throw new Exception($"City with id {city.Id} does not exist");
            
            return await _cityRepository.UpdateCity(city);
        }

        public Task<bool> RemoveCities(List<Guid> cities)
        {
            if(cities == null || cities.Count == 0)
                throw new ArgumentNullException(nameof(cities));
            
            return _cityRepository.RemoveCities(cities);
        }
    }
}