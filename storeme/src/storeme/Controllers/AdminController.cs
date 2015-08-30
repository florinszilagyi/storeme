using System.Web.Mvc;
using storeme.Data;
using storeme.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System.Web.Caching;
using storeme.Code;

namespace storeme.Controllers
{
    public class AdminController : Controller
    {
        private readonly IDashboardRepository repository = EntityResolver.ResolveRepository();

        // GET: Home
        [HttpPost]
        [OutputCache(Duration = 3600 * 12, VaryByParam = "none")]
        public async Task<FormattedJsonResult> GetData()
        {
            var vm = new Latest30Days();
            var byDate = await repository.GetDashboardsByDate();
            vm.Dates = byDate.Select(s => s.Item1).ToList();
            vm.Dashboards = byDate.Select(s => s.Item2).ToList();
            vm.Files = byDate.Select(s => s.Item3).ToList();

            return new FormattedJsonResult(vm);
        }

        protected override void Dispose(bool disposing)
        {
            this.repository.Dispose();
            base.Dispose(disposing);
        }
    }
}