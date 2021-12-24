using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BLL.DTO;
using BLL.Infrastructure;
using BLL.Interfaces;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    public class AccountController : Controller
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

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model)
        {
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO { Email = model.Email, Password = model.Password };
                ClaimsIdentity claim = await UserService.Authenticate(userDto);
                if (claim == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    UserDTO u =  await UserService.FindByEmail(userDto.Email);
                    HttpCookie cookie = new HttpCookie("user", u.Id);
                    cookie.Expires = DateTime.Now.AddMinutes(5);
                    Response.Cookies.Add(cookie);
                    return RedirectToAction("Profile","Member");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            await SetInitialDataAsync();
            if (ModelState.IsValid)
            {
                UserDTO userDto = new UserDTO
                {
                    Email = model.Email,
                    Password = model.Password,
                    Address = model.Address,
                    Name = model.Name,
                    Role = "user",
                    Age = model.Age,
                    Info = model.Info
                };
                OperationDetails operationDetails = await UserService.Create(userDto);
                if (operationDetails.Succedeed)
                    return View("Login");
                else
                    ModelState.AddModelError(operationDetails.Property, operationDetails.Message);
            }
            return View(model);
        }

        private async Task SetInitialDataAsync()
        {
            List<UserDTO> users = new List<UserDTO>
            {
                new UserDTO
                {
                    Email = "test@mail.ru",
                    UserName = "test@mail.ru",
                    Password = "test666",
                    Name = "Test Admin",
                    Address = "ул. Пушкина, д.47, кв.47",
                    Role = "admin",
                    Age = 15,
                    Info = "Test Admin for SN"
                },
                new UserDTO
                {
                    Email = "aboba@mail.ru",
                    UserName = "aboba@mail.ru",
                    Password = "aboba666",
                    Name = "Test User",
                    Address = "Samara",
                    Role = "user",
                    Age = 54,
                    Info = "Test User for SN"
                }
            };
            List<string> roles = new List<string> {"user", "admin"};
            await UserService.SetInitialData(users,roles);
        }
    }
}
