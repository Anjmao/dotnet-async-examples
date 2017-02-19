using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webapi_samples.services;

namespace webapi_samples.Contollers
{
    public class DemoController : Controller
    {
        private static MyDataService _dataService => new MyDataService();

        [HttpGet]
        [Route("api/users")]
        public IEnumerable<User> Get()
        {
            var result = _dataService.GetUsers();
            return result;
        }

        [HttpGet]
        [Route("api/users/async")]
        public async Task<IEnumerable<User>> GetAsync()
        {
            var result = await _dataService.GetUsersAsync();
            return result;
        }
    }

}