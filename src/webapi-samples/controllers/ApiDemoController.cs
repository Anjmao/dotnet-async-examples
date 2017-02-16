using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConsoleApplication.Contollers
{
    public class DemoController : Controller
    {
        [HttpGet]
        [Route("api/demo")]
        public async Task<IEnumerable<User>> Get()
        {
            var service = new MyDataService();
            return await service.GetUsersAsync();
        }
    }

    public class MyDataService
    {
        public async Task<HttpResponseMessage[]> GetAsync()
        {
            var requests = new List<Request>
            {
                new Request("http://www.google.com"),
                new Request("http://www.bing.com"),
                new Request("http://www.yandex.com")
            };

            var tasks = requests.Select(ProcessApiRequestAsync);
            return await Task.WhenAll(tasks);
        }

        private Func<IDbConnection> GetConnection => () => new MySql.Data.MySqlClient.MySqlConnection("Server=127.0.0.1;Database=demo;Uid=root;Pwd=admin;SslMode=None;");

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            using(var connection = GetConnection()) 
            {
                return await connection.QueryAsync<User>("SELECT * FROM user");
            }
        }

        public async Task SeedData()
        {
            using(var connection = GetConnection())
            {
                for(var i = 0; i < 100; i++) 
                {
                    await connection.ExecuteAsync("INSERT user(id, firstname, lastname) VALUES(@Id, @FirstName, @LastName)", new {
                        Id = Guid.NewGuid(),
                        FirstName = "Name" + i,
                        LastName = "LastName" + i
                    });
                }
            }
        }

        public async Task<HttpResponseMessage> ProcessApiRequestAsync(Request request)
        {
            using (var httpClient = new HttpClient())
            {
                Console.WriteLine($"Starting {request.Url} on thread {ThreadId}");

                var response = await httpClient.GetAsync(request.Url);

                Console.WriteLine($"End request {request.Url} on thread {ThreadId}");
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
        public int Value { get; set; }

        public Request(string url)
        {
            Url = url;
        }
    }

    public class User 
    {
        public string Id {get;set;}
        public string FirstName{get;set;}
        public string LastName{get;set;}
    }
}