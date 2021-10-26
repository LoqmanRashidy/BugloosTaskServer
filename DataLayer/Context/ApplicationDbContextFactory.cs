using DataLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace BaseRad.EntityFrameworkCore
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<ApplicationDbContext> builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            string basePath = Directory.GetCurrentDirectory();
            var configuration = new ConfigurationBuilder()
                               .SetBasePath(basePath)
                               .AddJsonFile("appsettings.json")
                               .Build();
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                  .Replace("|DataDirectory|", Path.Combine(basePath));
            builder.UseSqlServer(connectionString);
            return new ApplicationDbContext(builder.Options);

        }
    }
}
