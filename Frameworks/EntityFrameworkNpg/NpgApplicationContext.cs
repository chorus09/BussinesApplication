using BussinesApplication.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;

namespace BussinesApplication.Frameworks.EntityFrameworkNpg; 
public class NpgApplicationContext : DbContext {
    public DbSet<Product> Products { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        string jsonFilePath = "appsettings.json";
        string jsonString = File.ReadAllText(jsonFilePath);
        dynamic settings = JsonConvert.DeserializeObject(jsonString)!;
        string postgreSQLConnection = settings?.ConnectionStrings?.PostgreSQLConnection!;

        if (!string.IsNullOrEmpty(postgreSQLConnection)) {
            optionsBuilder.UseNpgsql(postgreSQLConnection);
        } else {
            throw new InvalidOperationException("PostgreSQL connection string is not set in appsettings.json.");
        }
    }
}