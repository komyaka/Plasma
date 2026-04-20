using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Plazma.Controllers.Users;


namespace Plazma.Controllers
{
    public class CutSawController : Controller
    {
        // GET: CutSaw
        public _user currentuser;
        public Users users = new Users();

        public ActionResult CutSaw()
        {
            currentuser = users.getCurrentUser();
            ViewBag.User = currentuser;
            return View();
        }
    }
}