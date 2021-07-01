using System.Threading;
using System.Threading.Tasks;
using ESFA.DC.DashBoard.Models;

namespace ESFA.DC.DashBoard.Interface
{
    public interface IDashBoardService
    {
        Task<DashBoardModel> ProvideAsync(CancellationToken cancellationToken);
    }
}