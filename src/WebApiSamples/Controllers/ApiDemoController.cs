using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApiSamples.Services;

using static System.Console;
using static System.Threading.Thread;

namespace WebApiSamples.Contollers
{
    public class DemoController : Controller
    {
        private static MyDataService _dataService => new MyDataService();

        [HttpGet]
        [Route("api/users")]
        public IEnumerable<User> Get()
        {
            return _dataService.GetUsers();
        }

        [HttpGet]
        [Route("api/users/async")]
        public async Task<IEnumerable<User>> GetAsync()
        {
            WriteLine($"Users start on thread {CurrentThread.ManagedThreadId}");
            var result = await _dataService.GetUsersAsync();
            WriteLine($"Users end on thread {CurrentThread.ManagedThreadId}");
            WriteLine("---------------------");
            return result;
        }

        [HttpGet]
        [Route("api/users/deadlock")]
        public IEnumerable<User> Deadlock()
        {
            //AspNetSynchronizationContext
            //HttpContext.Current
            var users = _dataService.GetUsersAsync().Result;
            return users;
        }
    }

}