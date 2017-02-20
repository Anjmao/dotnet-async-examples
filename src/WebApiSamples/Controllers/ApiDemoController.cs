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
            var result = _dataService.GetUsers();
            var content = _dataService.GetWebContent(new Request("http://webcache.googleusercontent.com/search?q=cache:https://en.wikipedia.org/wiki/Main_Page"));
            return result;
        }

        [HttpGet]
        [Route("api/users/async")]
        public async Task<IEnumerable<User>> GetAsync()
        {
            var usersTask = _dataService.GetUsersAsync();
            var wikiTask = _dataService.GetWebContentAsync(new Request("http://webcache.googleusercontent.com/search?q=cache:https://en.wikipedia.org/wiki/Main_Page"));
            var content = await wikiTask;
            return await usersTask;
        }
    }

}