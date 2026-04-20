using System.Web.Mvc;

namespace Plazma.Controllers
{
   // [Authorize]
    public abstract class BaseController : Controller
    {
        protected readonly Users users = new Users();
        protected Users._user currentuser;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            currentuser = users.getCurrentUser();
            ViewBag.User = currentuser;
            base.OnActionExecuting(filterContext);
        }
    }
}
