using DataModels;
using DataModels.Requests;
using Microsoft.EntityFrameworkCore;

namespace Questly.Repositories;

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

    public async Task<bool> DoesCityNameExistAsync(string name, Guid? excludeCityId = null)
    {
        // Сравнение приводим к нижнему регистру: в Postgres обычное сравнение строк
        // регистрозависимо, и без этого «Чебоксары» и «чебоксары» заедут как два города.
        var normalized = name.Trim().ToLower();

        return await _databaseConnection.Cities
            .AsNoTracking()
            .AnyAsync(q => q.Name.ToLower() == normalized
                           && (excludeCityId == null || q.Id != excludeCityId));
    }

    public async Task<List<City>> GetCitiesList()
    {
        return await _databaseConnection.Cities.ToListAsync();
    }

    public async Task<City> GetCityInfo(Guid cityId)
    {
        var city = await _databaseConnection.Cities.FirstOrDefaultAsync(q=>q.Id == cityId);
        if(city == null)
            throw new KeyNotFoundException($"City with ID {cityId} not found");
        
        return city;
    }

    public async Task<City> CreateCityAsync(CityRequest request)
    {
        var city = new City
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            Lat = request.Lat,
            Lon = request.Lon
        };

        _databaseConnection.Cities.Add(city);
        await _databaseConnection.SaveChangesAsync();

        return city;
    }

    public async Task<City> UpdateCityAsync(Guid cityId, CityRequest request)
    {
        var city = await _databaseConnection.Cities.FirstOrDefaultAsync(q => q.Id == cityId);
        if (city == null)
            throw new KeyNotFoundException($"City with ID {cityId} not found");

        city.Name = request.Name.Trim();
        city.Description = request.Description?.Trim();
        city.Lat = request.Lat;
        city.Lon = request.Lon;

        await _databaseConnection.SaveChangesAsync();

        return city;
    }
}
