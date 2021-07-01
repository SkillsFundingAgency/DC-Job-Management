using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DashBoard.Interface;
using ESFA.DC.DashBoard.Models;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.Job.WebApi.Controllers
{
    [Route("api/dashboard")]
    public sealed class DashBoardController : Controller
    {
        private readonly IDashBoardService _dashBoardService;

        public DashBoardController(
            IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        [HttpGet("stats")]
        public async Task<DashBoardModel> Stats()
        {
            return await _dashBoardService.ProvideAsync(CancellationToken.None);
        }
    }
}