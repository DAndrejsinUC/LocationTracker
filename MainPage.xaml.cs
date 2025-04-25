using System.Diagnostics;
using LocationTracker.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
namespace LocationTracker;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        CenterMap();
        _ = RequestPermissionsAsync(); // fire and forget
        LoadSavedPinsAsync();
        StartLocationTracking();
    }

    /// <summary>
    /// Starts a periodic timer to track the user's location every 10 seconds.
    /// Saves the location to the database and adds a pin to the map for each tracked location.
    /// </summary>
    private void StartLocationTracking()
    {
        var timer = new System.Threading.PeriodicTimer(TimeSpan.FromSeconds(10));

        Task.Run(async () =>
        {
            while (await timer.WaitForNextTickAsync())
            {
                try
                {
                    var location = await Geolocation.GetLocationAsync(
                        new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10)));

                    if (location != null)
                    {
                        var data = new LocationData
                        {
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            Timestamp = DateTime.UtcNow
                        };

                        await App.Database.SaveLocationAsync(data);
                        Debug.WriteLine($"📍 Saved: {data.Latitude}, {data.Longitude}");

                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            UserMap.Pins.Add(new Pin
                            {
                                Label = "Tracked Location",
                                Location = new Location(data.Latitude, data.Longitude)
                            });
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Location error: {ex.Message}");
                }
            }
        });
    }

    /// <summary>
    /// Requests location permissions from the user.
    /// Displays an alert if the permission is not granted.
    /// </summary>
    private async Task RequestPermissionsAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationAlways>();
        }

        if (status != PermissionStatus.Granted)
        {
            await DisplayAlert("Permissions", "Location permission is required to track your position.", "OK");
        }
    }

    /// <summary>
    /// Centers the map on the user's current location.
    /// If the location cannot be retrieved, logs an error message.
    /// </summary>
    private async void CenterMap()
    {
        try
        {
            var location = await Geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium));

            if (location != null)
            {
                var center = new Location(location.Latitude, location.Longitude);
                UserMap.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(2)));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to center map: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads previously saved locations from the database and adds pins to the map for each location.
    /// </summary>
    private async Task LoadSavedPinsAsync()
    {
        var locations = await App.Database.GetAllLocationsAsync();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            foreach (var location in locations)
            {
                UserMap.Pins.Add(new Microsoft.Maui.Controls.Maps.Pin
                {
                    Location = new Location(location.Latitude, location.Longitude),
                    Label = "Previous Location",
                    Address = location.Timestamp.ToLocalTime().ToString("g")
                });
            }
        });
    }
}
