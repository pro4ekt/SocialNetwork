using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using BLL.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace SocialNetwork
{
    public class BanCheck : FilterAttribute, IAuthenticationFilter
    {
        private IUserService UserService => HttpContext.Current.GetOwinContext().GetUserManager<IUserService>();
        public void OnAuthentication(AuthenticationContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            var u = UserService.FindById(HttpContext.Current.User.Identity.GetUserId()).Result;
            if (u.Banned)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary {
                        { "controller", "Account" }, { "action", "Login" }
                });
            }
            if (user == null || !user.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary {
                        { "controller", "Client" }, { "action", "Profile" }
                    });
            }
        }
    }
}