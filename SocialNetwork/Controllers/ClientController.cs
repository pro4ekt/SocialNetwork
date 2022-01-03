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
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class ClientController : Controller
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
        public ActionResult Home()
        {
            return View();
        }

        [Authorize]
        public new async Task<ActionResult> Profile()
        {
            HttpCookie cookie = Request.Cookies["user"];
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
            if (userToFind.ToLower().Contains("@"))
            {
                UserDTO u = await UserService.FindByEmail(userToFind);
                return RedirectToAction("FindUser", u);
            }
            else
            {
                UserDTO u = await UserService.FindByName(userToFind);
                return RedirectToAction("FindUser", u);
            }
        }

        [Authorize]
        public async Task<ActionResult> EditUserProfile()
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO u = await UserService.FindById(cookie.Value);
            return View(u);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUserProfile(UserDTO model)
        {
            HttpCookie cookie = Request.Cookies["user"];
            bool b = await UserService.EditProfile(cookie.Value,model.UserName,model.Email,model.Info,model.Address,model.Age);
            if (b == false)
            {
                return RedirectToAction("EditUserProfile");
            }
            return View("Home");
        }

        [Authorize]
        public async Task<ActionResult> AddFriend(string friendId)
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO u2 = await UserService.FindById(friendId);
            bool b = await UserService.AddFriend(cookie.Value, u2.Id);
            if(b)
                return RedirectToAction("Home");
            return RedirectToAction("FindUser",u2);
        }

        [Authorize]
        public async Task<ActionResult> RemoveFriend(string friendId)
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO u2 = await UserService.FindById(friendId);
            bool b = await UserService.RemoveFriend(cookie.Value, u2.Id);
            if (b)
                return RedirectToAction("Home");
            return RedirectToAction("YourFriends");
        }

        [Authorize]
        public async Task<ActionResult> YourFriends()
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO  u1 = await UserService.FindById(cookie.Value);
            List<UserDTO> users = new List<UserDTO>();
            foreach (var f in u1.Friends)
            {
                users.Add(await UserService.FindById(f.FriendId));
            }
            return View(users);
        }

        [Authorize]
        public async Task<ActionResult> Chat(string friendId)
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO u1 = await UserService.FindById(cookie.Value);
            ViewBag.FriendId = friendId;
            return View();
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> BanUser(string userId)
        {
            await UserService.BanUser(userId);
            return RedirectToAction("Home");
        }
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UnBanUser(string userId)
        {
            await UserService.UnBanUser(userId);
            return RedirectToAction("Home");
        }
    }
}
