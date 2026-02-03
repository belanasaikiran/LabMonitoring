using System;
using Microsoft.EntityFrameworkCore;

namespace LabServer{
    public class MachineLog{
        public int Id {get; set;}
        public string MacAddress {get; set;}
        public string CpuLoad {get; set; }
        public string FireWallStatus {get; set; }
        public DateTime TimeStamp {get; set;}
    }

    // this is the bridge to database
    public class LabContext : DbContext
    {
        public DbSet<MachineLog> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = @"C:\Users\saiki\Documents\dotnet\lab_inventory.db";
            options.UseSqlite($"Data Source={dbPath}");
        }
    }
} 