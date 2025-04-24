using System;
using System.IO;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using LocationTracker.Services;

namespace LocationTracker
{
    public partial class App : Application
    {
        private static LocationDatabase _database;

        public static LocationDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    string dbPath = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        "locations.db3");

                    _database = new LocationDatabase(dbPath);
                }
                return _database;
            }
        }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell(); // or MainPage() if you're not using AppShell
        }
    }
}
