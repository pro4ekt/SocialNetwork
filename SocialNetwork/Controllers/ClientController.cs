﻿using System;
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
        private IUserService UserService => HttpContext.GetOwinContext().GetUserManager<IUserService>();

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        [Authorize]
        [BanCheck]
        public new async Task<ActionResult> Profile()
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO u = await UserService.FindById(cookie.Value);
            return View(u);
        }

        [Authorize]
        [BanCheck]
        public async Task<ActionResult> FindUser(List<UserDTO> u)
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO user = await UserService.FindById(cookie.Value);
            ViewBag.Email = user.Email;
            u = (List<UserDTO>) TempData["List"];
            return View(u);
        }

        [Authorize]
        [BanCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FindUser(string userToFind)
        {
            if (userToFind.ToLower().Contains("@"))
            {
                UserDTO user = await UserService.FindByEmail(userToFind);
                List<UserDTO> uL = new List<UserDTO>();
                uL.Add(user);
                TempData["List"] = uL;
                return RedirectToAction("FindUser");
            }
            if (userToFind.Equals(""))
            {
                List<UserDTO> uL = await UserService.FindAll();
                TempData["List"] = uL;
                return RedirectToAction("FindUser");
            }
            int result;
            if (int.TryParse(userToFind, out result))
            {
                var uL = await UserService.FindByAge(result);
                TempData["List"] = uL;
                return RedirectToAction("FindUser");
            }
            else
            {
                UserDTO user = await UserService.FindByName(userToFind);
                List<UserDTO> uL = new List<UserDTO>();
                uL.Add(user);
                TempData["List"] = uL;
                return RedirectToAction("FindUser");
            }
        }

        [Authorize]
        [BanCheck]
        public async Task<ActionResult> EditUserProfile()
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO u = await UserService.FindById(cookie.Value);
            return View(u);
        }

        [Authorize]
        [BanCheck]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditUserProfile(UserDTO model)
        {
            HttpCookie cookie = Request.Cookies["user"];
            bool b = await UserService.EditProfile(cookie.Value,model.UserName,model.Email,model.Info,model.Address,model.Age, await UserService.FindById(cookie.Value));
            if (b == false)
            {
                return RedirectToAction("EditUserProfile");
            }
            return RedirectToAction("Profile");
        }

        [Authorize]
        [BanCheck]
        public async Task<ActionResult> AddFriend(string friendId)
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO u2 = await UserService.FindById(friendId);
            bool b = await UserService.AddFriend(cookie.Value, u2.Id);
            if(b)
                return RedirectToAction("YourFriends");
            return RedirectToAction("FindUser",u2);
        }

        [Authorize]
        [BanCheck]
        public async Task<ActionResult> RemoveFriend(string friendId)
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO u2 = await UserService.FindById(friendId);
            bool b = await UserService.RemoveFriend(cookie.Value, u2.Id);
            if (b)
                return RedirectToAction("YourFriends");
            return RedirectToAction("YourFriends");
        }

        [Authorize]
        [BanCheck]
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
        [BanCheck]
        public async Task<ActionResult> Chat(string friendId)
        {
            HttpCookie cookie = Request.Cookies["user"];
            UserDTO u1 = await UserService.FindById(cookie.Value);
            ViewBag.FriendId = friendId;
            return View();
        }

        [Authorize]
        [BanCheck]
        public async Task<ActionResult> YourMessages(string id)
        {
            if (id == null)
            {
                HttpCookie cookie = Request.Cookies["user"];
                UserDTO u1 = await UserService.FindById(cookie.Value);
                return View(u1.Messages);
            }
            else
            {
                var l = await UserService.FindById(id);
                return View(l.Messages);
            }
        }

        [Authorize]
        [BanCheck]
        public async Task<ActionResult> RemoveMessage(string messageId,string id, string receiverId, string receiverName ,DateTime date, string text)
        {
            MessagesDTO message = new MessagesDTO
            {
                MessageId = messageId,
                Id = id,
                ReceiverId = receiverId,
                ReceiverName = receiverName,
                Text = text,
                DateTime = date
            };
            await UserService.RemoveMessage(message);
            return RedirectToAction("YourMessages");
        }

        [Authorize(Roles = "admin")]
        public async Task<ActionResult> BanUser(string userId)
        {
            await UserService.BanUser(userId);
            return RedirectToAction("FindUser");
        }
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> UnBanUser(string userId)
        {
            await UserService.UnBanUser(userId);
            return RedirectToAction("FindUser");
        }
    }
}
