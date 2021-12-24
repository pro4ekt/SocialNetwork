using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private IUserService UserService
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<IUserService>();
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [Authorize] 
        public async Task<ActionResult> Profile()
        {
            HttpCookie cookie = Request.Cookies["user"];
            cookie.Expires = DateTime.Now.AddMinutes(5);
            UserDTO u = await UserService.FindById(cookie.Value);
            return View(u);
        }

        [Authorize]
        public ActionResult FindUser(UserDTO u)
        {
            return View(u);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FindUser(string userToFind)
        {
            UserDTO u = await UserService.FindByEmail(userToFind);
            return View("FindUser", u);
        }
    }
}
