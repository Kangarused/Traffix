using System;
using System.Collections.Generic;
using FluentMigrator;
using FluentMigrator.Runner.Extensions;
using Traffix.Common.Model;
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
                LinkId = "Tiger Brennan",
                SpeedLimit = 100
            },
            new MockTrafficMeter {
                Id = 2,
                Name = "Meter 2",
                Region = "Northern Territory",
                Latitude = -12.435336,
                Longitude = 130.897257,
                Congestion = (int)CongestionTypes.Low,
                DateActive = DateTime.Now,
                LinkId = "Tiger Brennan",
                SpeedLimit = 100
            },
            new MockTrafficMeter {
                Id = 3,
                Name = "Meter 3",
                Region = "Northern Territory",
                Latitude = -12.425474,
                Longitude = 130.886922,
                Congestion = (int)CongestionTypes.Low,
                DateActive = DateTime.Now,
                LinkId = "Winnelle",
                SpeedLimit = 80
            },
            new MockTrafficMeter {
                Id = 4,
                Name = "Meter 4",
                Region = "Northern Territory",
                Latitude = -12.425228,
                Longitude = 130.881365,
                Congestion = (int)CongestionTypes.Low,
                DateActive = DateTime.Now,
                LinkId = "Winnelle",
                SpeedLimit = 80
            },
            new MockTrafficMeter {
                Id = 5,
                Name = "Meter 5",
                Region = "Northern Territory",
                Latitude = -12.425375,
                Longitude = 130.871489,
                Congestion = (int)CongestionTypes.Low,
                DateActive = DateTime.Now,
                LinkId = "Winnelle",
                SpeedLimit = 80
            },
            new MockTrafficMeter {
                Id = 6,
                Name = "Meter 6",
                Region = "Northern Territory",
                Latitude = -12.426721,
                Longitude = 130.863247,
                Congestion = (int)CongestionTypes.Low,
                DateActive = DateTime.Now,
                LinkId = "Winnelle",
                SpeedLimit = 80
            },
            new MockTrafficMeter {
                Id = 7,
                Name = "Meter 7",
                Region = "Northern Territory",
                Latitude = -12.429246,
                Longitude = 130.857377,
                Congestion = (int)CongestionTypes.Low,
                DateActive = DateTime.Now,
                LinkId = "Winnelle",
                SpeedLimit = 80
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
                Secret = "#123Traffix654@",
                ApplicationType = "AngularJS Front-End Application"
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
                    meter.LinkId,
                    meter.SpeedLimit
                });
            }
        }

        private void GenerateLogs()
        {
            Random rand = new Random();

            foreach (MockTrafficMeter meter in _trafficMeters)
            {
                for (int x = 0; x < 5; x++)
                {
                    var randomSpeed = rand.Next(50, 130);
                    Insert.IntoTable("TrafficLogs").Row(new
                    {
                        MeterId = meter.Id,
                        Time = DateTime.Now,
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