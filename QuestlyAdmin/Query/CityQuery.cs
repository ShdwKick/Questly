using DataModels;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authorization;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Queries
{
    [ExtendObjectType(typeof(Query))]
    public class CityQuery
    {
        private readonly ICityService _cityService;

        public CityQuery(ICityService cityService)
        {
            _cityService = cityService;
        }

        [Authorize]
        [GraphQLDescription("AUTHORIZE-Получить данные о городе")]
        public async Task<City> GetCityInfo(Guid cityId)
        {
            return await _cityService.GetCityInfo(cityId);
        }
    
        [Authorize]
        [GraphQLDescription("AUTHORIZE-Получить список городов")]
        public async Task<List<City>> GetCitiesList()
        {
            return await _cityService.GetCitiesList();
        }
    }
}