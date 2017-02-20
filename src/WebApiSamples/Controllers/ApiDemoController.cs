using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApiSamples.Services;

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
            return await _dataService.GetUsersAsync();
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