using BussinesApplication.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.IO;

namespace BussinesApplication.Frameworks.EntityFramework;
public class ApplicationContext : DbContext {
    public DbSet<Client> Clients { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        string jsonFilePath = "appsettings.json";
        string jsonString = File.ReadAllText(jsonFilePath);
        dynamic settings = JsonConvert.DeserializeObject(jsonString)!;
        string mssqlConnection = settings?.ConnectionStrings?.MSSQLConnection!;

        if (!string.IsNullOrEmpty(mssqlConnection)) {
            optionsBuilder.UseSqlServer(mssqlConnection);
        } else {
            throw new InvalidOperationException("MSSQL connection string is not set in appsettings.json.");
    }
}