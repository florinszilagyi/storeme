using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using storeme.Data;
using storeme.Data.Encryption;
using MongoDB.Driver.Linq;
using storeme.Code;
using storeme.Data.Model;
using storeme.ViewModels;

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

        

       
    }
}