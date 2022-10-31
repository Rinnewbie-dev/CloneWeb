using CookieAuthentication.Models;
using EntityDataModel.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CloneWeb.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly EntityDataContext _context;
        public AuthenticationController(IConfiguration configuration , EntityDataContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [AllowAnonymous]
        public IActionResult Login(string ReturnUrl = "")
        {

            LoginModel objLoginModel = new LoginModel();
            objLoginModel.ReturnUrl = ReturnUrl;
            return View(objLoginModel);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel objLoginModel)
        {
            if (ModelState.IsValid)
            {
                var pashMD5 = GetMd5Sum(objLoginModel.Password);
                var user = _context.User.Where(x => x.UserName == objLoginModel.UserName && x.Password == pashMD5).FirstOrDefault();
                if (user != null)
                {
                    if (string.IsNullOrEmpty(objLoginModel.ReturnUrl))
                    {
                        objLoginModel.ReturnUrl = "/";
                    }
                    
                    var claims = new List<Claim>() {
                        new Claim("UserId", user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Role, user.Role),
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties()
                    {
                        IsPersistent = objLoginModel.RememberMe
                    });
                    return LocalRedirect(objLoginModel.ReturnUrl);
                }
                else
                {
                    ViewBag.Message = "Wrong username or password";
                    return View(user);
                }
            }
            return View(objLoginModel);
        }
        [AllowAnonymous]
        public IActionResult Register(string ReturnUrl = "")
        {

            LoginModel objLoginModel = new LoginModel();
            objLoginModel.ReturnUrl = ReturnUrl;
            return View(objLoginModel);
        }
        #region GetRedirectUrl
        private string GetRedirectUrl(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return Url.Action("Index", "Home");
            }

            return returnUrl;
        }
        #endregion GetRedirectUrl
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return LocalRedirect("/");
        }
        public string GetMd5Sum(string str)
        {
          

            Encoder enc = System.Text.Encoding.Unicode.GetEncoder();
            byte[] unicodeText = new byte[str.Length * 2];

            enc.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);


            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] result = md5.ComputeHash(unicodeText);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < result.Length; i++)
            {

                sb.Append(result[i].ToString("X2"));

            }
            // And return it
            return sb.ToString();
        }
    }
}
