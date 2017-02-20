using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;

using MySQL.Data.EntityFrameworkCore.Extensions;

namespace WebApiSamples.Services
{
    public class MyDataService
    {
        private Func<IDbConnection> GetDapperConnection => () => new MySql.Data.MySqlClient.MySqlConnection("Server=127.0.0.1;Database=demo;Uid=root;Pwd=admin;SslMode=None;");

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            using (var connection = GetDapperConnection())
            {
                var result = await connection.QueryAsync<User>("SELECT * FROM user");
                return result;
            }
        }

        public IEnumerable<User> GetUsers()
        {
            using (var connection = GetDapperConnection())
            {
                var result = connection.Query<User>("SELECT * FROM user");
                return result;
            }
        }

        public async Task<HttpResponseMessage> GetWebContentAsync(Request request)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(request.Url).ConfigureAwait(false);
                return response;
            }
        }

        public HttpResponseMessage GetWebContent(Request request)
        {
            using (var httpClient = new HttpClient())
            {
                var response = httpClient.GetAsync(request.Url).Result;
                return response;
            }
        }

        public async Task SeedData()
        {
            using (var connection = GetDapperConnection())
            {
                for (var i = 0; i < 100; i++)
                {
                    await connection.ExecuteAsync("INSERT user(id, firstname, lastname) VALUES(@Id, @FirstName, @LastName)", new
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Name" + i,
                        LastName = "LastName" + i
                    });
                }
            }
        }
    }

    public class Job
    {
        public Job(string id, int time)
        {
            Id = id;
            Seconds = time;
        }
        public string Id { get; }
        public int Seconds { get; }
    }

    public class Request
    {
        public string Url { get; }

        public Request(string url)
        {
            Url = url;
        }
    }

    [Table("user")]
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(@"Server=127.0.0.1;Database=demo;Uid=root;Pwd=admin;SslMode=None;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("user");
        }
    }
}