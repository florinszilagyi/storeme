using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver;
using storeme.Data;
using storeme.Data.Encryption;
using MongoDB.Driver.Linq;
using storeme.Data.Model;

namespace storeme.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        private DashboardContext context = new DashboardContext();

        /// <summary>
        /// Uploads this instance.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [HttpPost]
        public async Task<JsonResult> Index(HttpPostedFileBase file)
        {
            var filename = file.FileName;
            var buffer = new byte[file.ContentLength];
            file.InputStream.Read(buffer, 0, file.ContentLength);

            var encryptor = new DashboardEncryptor(buffer, filename);
            var dashboard = await encryptor.Encrypt();
            await context.Dashboards.InsertOneAsync(dashboard);

            var passCode = encryptor.PassCode;
            return Json(new { PassCode = passCode });
        }

        [HttpGet]
        public async Task<JsonResult> File(string passCode)
        {
            var dashboard = await context.Dashboards.Find(f => f.Key == passCode).FirstOrDefaultAsync();

            var decryptor = new DashboardDecryptor(dashboard);
            var file = decryptor.DecryptFile();

            return Json(new {file.Name});
        }
    }
}