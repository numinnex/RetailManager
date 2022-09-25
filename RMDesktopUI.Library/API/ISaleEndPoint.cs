using RMDesktopUI.Library.Models;
using System.Threading.Tasks;

namespace RMDesktopUI.Library.API
{
    public interface ISaleEndPoint
    {
        Task PostSale(SaleModel sale);
    }
}