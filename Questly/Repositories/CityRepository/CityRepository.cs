using DataModels;
using Microsoft.EntityFrameworkCore;
using Questly.DataBase;

namespace Questly.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly DatabaseContext _databaseConnection;

        public CityRepository(DatabaseContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<bool> DoesCityExist(string name)
        {
            return await _databaseConnection.Cities.AnyAsync(q=>q.Name.Equals(name));
        }

        public async Task<bool> DoesCityExist(Guid cityId)
        {
            return await _databaseConnection.Cities.AnyAsync(q=>q.Id == cityId);
        }

        public async Task<List<City>> GetCitiesList()
        {
            return await _databaseConnection.Cities.ToListAsync();
        }

        public async Task<City> GetCityInfo(Guid cityId)
        {
            var city = await _databaseConnection.Cities.FirstOrDefaultAsync(q=>q.Id == cityId);
            if(city == null)
                throw new Exception($"City with id:{cityId} not found");
        
            return city;
        }
    }
}