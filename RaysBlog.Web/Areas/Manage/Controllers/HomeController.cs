using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaysBlog.Repository;
using RaysBlog.Web.Models;

namespace RaysBlog.Web.Areas.Manage.Controllers
{
    [Area("Manage")]
    [Route("ray")]
    [Authorize(Roles = "admin,system")]
    public class HomeController : Controller
    {
        private readonly UserInfoRepository userInfoRepository;
        public HomeController()
        {
            userInfoRepository = new UserInfoRepository();
        }
        [Route("index")]
        public IActionResult Index()
        {        
            return View();
        }
        [Route("login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View("login");
        }
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Login(string loginName, string password)
        {
            //需要加入登录次数限制
            if (string.IsNullOrEmpty(loginName) && string.IsNullOrEmpty(password))
            {
                return Json(new { code = 1, msg = "失败" });
            }
            else
            {
                var user =await userInfoRepository.GetAsync(loginName, MD5Hash(password));
                if (user != null)
                {
                    //用户标识
                    var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    identity.AddClaim(new Claim(ClaimTypes.Sid,loginName));
                    identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Role,user.Permission.PermissionName));
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                    //@User.Claims.SingleOrDefault(s=>s.Type==System.Security.Claims.ClaimTypes.Sid).Value 获取用户名
                    return Redirect("index");
                }
                return Redirect("Login");//Json(new { code = 1, msg = "失败" });
            }
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("login");
        }
        [HttpGet("Error")]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private  string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
    }
}