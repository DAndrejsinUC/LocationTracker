using System.Diagnostics;
using LocationTracker.Models;
using Microsoft.Maui.Controls.Maps;
namespace LocationTracker;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        //StartLocationTracking();
    }

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

                        // Optional: Add a pin for testing (will be replaced by heatmap later)
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
}