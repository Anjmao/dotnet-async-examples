using System;
using System.Collections.Generic;
using System.Threading;
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
            var result = await _dataService.GetUsersAsync();
            return result;
        }

        [HttpGet]
        [Route("api/long")]
        public string LongTask()
        {
            Thread.Sleep(1000);
            return "Long task done";
        }

        [HttpGet]
        [Route("api/long/async")]
        public async Task<string> LongAsyncTask()
        {
            await Task.Delay(1000);
            return "Long task done";
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