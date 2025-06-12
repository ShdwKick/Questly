using DataModels;
using DataModels.DTOs;
using Microsoft.EntityFrameworkCore;
using QuestlyAdmin.DataBase;

namespace QuestlyAdmin.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly DatabaseContext _databaseConnection;

        public CityRepository(DatabaseContext databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<bool> DoesCityExistAsync(string name)
        {
            return await _databaseConnection.Cities.AnyAsync(q=>q.Name.Equals(name));
        }

        public async Task<bool> DoesCityExistAsync(Guid cityId)
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

        public Task<bool> CreateCities(List<CityDTO> cities)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> UpdateCity(City city)
        {
            var cty = await _databaseConnection.Cities.FindAsync(city.Id);

            cty = city;
            _databaseConnection.Cities.Update(cty);
            
            var affectedRows = await _databaseConnection.SaveChangesAsync();

            return affectedRows > 0;
        }

        public async Task<bool> RemoveCities(List<Guid> citiesId)
        {
            var cities = await _databaseConnection.Cities.Where(q => citiesId.Contains(q.Id)).ToListAsync();
            if (cities.Count == 0)
                return false;
            
            _databaseConnection.Cities.RemoveRange(cities);
            
            var affectedRows = await _databaseConnection.SaveChangesAsync();

            return affectedRows > 0;
        }
    }
}