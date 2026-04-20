using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Plazma.Controllers;
using static Plazma.Controllers.Users;

namespace Plazma.Controllers
{
    public class ADMController : Controller
    {
        // GET: ADM
        public List<string> fileList = new List<string> { };
        public Users users = new Users();
        public _user currentuser;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult uploadCNC(IEnumerable<HttpPostedFileBase> uploads)
        {
            currentuser = users.getCurrentUser();
            ViewBag.User = currentuser;
            return View();
        }
    }
}