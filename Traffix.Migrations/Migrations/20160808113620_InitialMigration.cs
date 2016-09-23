using System;
using System.ComponentModel;
using Traffix.Migrations.Extensions;
using FluentMigrator;
using FluentMigrator.Expressions;

namespace Traffix.Migrations.Migrations
{
    [Migration(20160808113620)]
    public class InitialMigration : Migration
    {
        public override void Up()
        {
            Create.Table("AuthClient")
                .WithId()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("Secret").AsMaxString().NotNullable()
                .WithColumn("ApplicationType").AsString().NotNullable();

            Create.Table("TrafficMeters")
                .WithId()
                .WithColumn("Name").AsString().Nullable()
                .WithColumn("Region").AsString().NotNullable()
                .WithColumn("Latitude").AsDouble().NotNullable()
                .WithColumn("Longitude").AsDouble().NotNullable()
                .WithColumn("Congestion").AsInt32().NotNullable()
                .WithColumn("DateActive").AsString().Nullable()
                .WithColumn("LinkId").AsString().NotNullable();

            Create.Table("TrafficLogs")
                .WithId()
                .WithColumn("MeterId").AsInt32().ForeignKey("TrafficMeters", "Id").NotNullable()
                .WithColumn("Time").AsDateTime().NotNullable()
                .WithColumn("Speed").AsInt32().NotNullable();

        }

        public override void Down()
        {
            Execute.DropTableIfExists("TrafficLogs");
            Execute.DropTableIfExists("TrafficMeters");
            Execute.DropTableIfExists("AuthClient");
        }
    }
}
