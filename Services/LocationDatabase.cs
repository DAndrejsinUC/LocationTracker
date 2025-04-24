using SQLite;
using LocationTracker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocationTracker.Services
{
    public class LocationDatabase
    {
        private readonly SQLiteAsyncConnection _database;

        public LocationDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<LocationData>().Wait();
        }

        public Task<int> SaveLocationAsync(LocationData location) =>
            _database.InsertAsync(location);

        public Task<List<LocationData>> GetLocationsAsync() =>
            _database.Table<LocationData>().ToListAsync();
    }
}
