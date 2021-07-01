using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/status")]
    public class StatusController : Controller
    {
        public async Task<IActionResult> Get()
        {
            return Ok("ok");
        }
    }
}