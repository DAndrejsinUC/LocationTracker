using SQLite;
using LocationTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationTracker.Services
{
    public class LocationDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        // Initializes the database connection and creates the LocationData table if it doesn't exist.
        public LocationDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<LocationData>().Wait();
        }

        // Saves a new location entry to the database.
        public Task<int> SaveLocationAsync(LocationData location) =>
            _database.InsertAsync(location);

        // Retrieves all location entries from the database.
        public Task<List<LocationData>> GetLocationsAsync() =>
            _database.Table<LocationData>().ToListAsync();

        // Retrieves all location entries from the database (identical to GetLocationsAsync).
        public Task<List<LocationData>> GetAllLocationsAsync()
        {
            return _database.Table<LocationData>().ToListAsync();
        }
    }
}
