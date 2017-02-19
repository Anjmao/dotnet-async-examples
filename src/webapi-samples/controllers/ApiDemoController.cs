using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using MySQL.Data.EntityFrameworkCore.Extensions;

namespace ConsoleApplication.Contollers
{
    public class DemoController : Controller
    {
        private int ThreadId => Thread.CurrentThread.ManagedThreadId;

        [HttpGet]
        [Route("api/users/async")]
        public async Task<IEnumerable<User>> GetAsync()
        {
            var service = new MyDataService();
            var result = await service.GetUsersAsync();
            return result;
        }

        [HttpGet]
        [Route("api/users")]
        public IEnumerable<User> Get()
        {
            var service = new MyDataService();
            var result = service.GetUsers();
            return result;
        }

        [HttpGet]
        [Route("api/users/ef/async")]
        public async Task<IEnumerable<User>> GetEfAsync()
        {
            var service = new MyDataService();
            var result = await service.GetEfUsersAsync();
            return result;
        }

        [HttpGet]
        [Route("api/users/ef")]
        public IEnumerable<User> GetEf()
        {
            var service = new MyDataService();
            var result = service.GetEfUsers();
            return result;
        }
    }

    public class MyDataService
    {
        private Func<IDbConnection> GetConnection => () => new MySql.Data.MySqlClient.MySqlConnection("Server=127.0.0.1;Database=demo;Uid=root;Pwd=admin;SslMode=None;");

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            using (var connection = GetConnection())
            {
                var result = await connection.QueryAsync<User>("SELECT * FROM user");
                return result;
            }
        }

        public async Task<IEnumerable<User>> GetEfUsersAsync()
        {
            using (var connection = new AppDbContext())
            {
                var result = await connection.Users.ToListAsync();
                return result;
            }
        }

        public IEnumerable<User> GetUsers()
        {
            using (var connection = GetConnection())
            {
                var result = connection.Query<User>("SELECT * FROM user");
                return result;
            }
        }

        public IEnumerable<User> GetEfUsers()
        {
            using (var connection = new AppDbContext())
            {
                var result = connection.Users.ToList();
                return result;
            }
        }

        public async Task SeedData()
        {
            using (var connection = GetConnection())
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

        private int ThreadId => Thread.CurrentThread.ManagedThreadId;
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