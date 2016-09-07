using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;
using Traffix.Common.Model;
using Traffix.Common.Providers;
using Traffix.Common.Utils.Extensions;
using Traffix.Migrations.Profiles.MockData;

namespace Traffix.Migrations.Profiles
{
    [Profile("Development")]
    public class DevelopmentProfile : Migration
    {
        private readonly List<MockTrafficMeter> _trafficMeters = new List<MockTrafficMeter>
        {
            new MockTrafficMeter {
                Id = 1,
                Name = "Meter 1",
                Region = "Northern Territory",
                Latitude = -12.450095,
                Longitude = 130.922595,
                Congestion = 1,
                DateActive = DateTime.Now,
                LinkId = "Tiger Brennan"
            },
            new MockTrafficMeter {
                Id = 2,
                Name = "Meter 2",
                Region = "Northern Territory",
                Latitude = -12.435321,
                Longitude = 130.897262,
                Congestion = 2,
                DateActive = DateTime.Now,
                LinkId = "Tiger Brennan"
            },
            new MockTrafficMeter {
                Id = 3,
                Name = "Meter 3",
                Region = "Northern Territory",
                Latitude = -12.433093,
                Longitude = 130.922256,
                Congestion = 2,
                DateActive = DateTime.Now,
                LinkId = "Winnelle"
            },
            new MockTrafficMeter {
                Id = 4,
                Name = "Meter 4",
                Region = "Northern Territory",
                Latitude = -12.425249,
                Longitude = 130.884486,
                Congestion = 0,
                DateActive = DateTime.Now,
                LinkId = "Winnelle"
            }
        }; 

        public override void Up()
        {
            LoadAuthClients();
            GenerateTrafficMeters();
            GenerateLogs();
        }

        private void LoadAuthClients()
        {
            Insert.IntoTable("AuthClient").Row(new
            {
                Name = "websiteAuth",
                Secret = new CryptoProvider(null).GetHash("SHRE%fZy4RL@8vButG#*%^KP6#yK6p"),
                ApplicationType = "AngularJS front-end Application",
                Active = true,
                AllowedOrigin = "http://localhost:2053"
            });
        }

        private void GenerateTrafficMeters()
        {
            foreach (MockTrafficMeter meter in _trafficMeters)
            {
                Insert.IntoTable("TrafficMeters").WithIdentityInsert().Row(new
                {
                    meter.Id,
                    meter.Name,
                    meter.Region,
                    meter.Latitude,
                    meter.Longitude,
                    meter.Congestion,
                    meter.DateActive,
                    meter.LinkId
                });
            }
        }

        private void GenerateLogs()
        {
            Random rand = new Random();
            int randomTime = 0;
            int randomSpeed = 0;

            foreach (MockTrafficMeter meter in _trafficMeters)
            {
                for (int x = 0; x < 20; x++)
                {
                    randomTime = rand.Next(1, 600);
                    randomSpeed = rand.Next(50, 130);
                    Insert.IntoTable("TrafficLogs").Row(new
                    {
                        MeterId = meter.Id,
                        Time = DateTime.Now.AddMinutes(randomTime),
                        Speed = randomSpeed
                    });
                }
            }
        }

        public override void Down()
        {
            Delete.FromTable("AuthClient").AllRows();
        }
    }
}