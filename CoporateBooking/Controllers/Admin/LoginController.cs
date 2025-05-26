using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using DomainLayer.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnionConsumeWebAPI.Extensions;
using ServiceLayer.Service.Interface;
using static System.Net.WebRequestMethods;

namespace OnionConsumeWebAPI.Controllers.Admin
{

   
    public class LoginController : Controller
    {

        //public IActionResult Index()
        //{
        //    return View();
        //}
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoginController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task<IActionResult> UserLogin(string returnUrl = null)
        {
            
            if (returnUrl == null)
            {
                HttpContext.Session.Clear();
            }
            
            ViewBag.ReturnUrl = returnUrl;
            return View();
            
        }

         [HttpPost]
        public async Task<IActionResult> UserLogin(string username, string password, string returnUrl = null)
        {
            using (HttpClient client = new HttpClient())
            {
                var loginRequest = new { Username = username, Password = password };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(AppUrlConstant.Corporatelogin, content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonResult = JObject.Parse(result);
                    var email = username; // or "email" if that's the correct key

                    // ✅ Save to session
                    HttpContext.Session.SetString("LoggedInEmail", email);


                      var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.Email, email),
                        new Claim(ClaimTypes.Authentication, password)
                    };

                    var identity = new ClaimsIdentity(claims, "Password");
                    var principal = new ClaimsPrincipal(identity);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        TempData["Email"] = email;
                        return RedirectToAction("Index", "FlightSearchIndex");
                       // return Redirect(returnUrl);
                    }
                    else
                    {
                        TempData["Email"] = email;
                        return RedirectToAction("Index", "FlightSearchIndex");


                    }
                    //return RedirectToAction("SearchResultFlight", "FlightSearchIndex", new { email = email });


                }

                ViewBag.ErrorMessage = "Invalid login credentials";
                return View();
            }



        }

        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Logout()
        {

            // Clear the session
            HttpContext.Session.Remove("LoggedInEmail");

            // Optionally, clear TempData if you need
            TempData.Remove("Email");

            return RedirectToAction("Index", "FlightSearchIndex");
        }


    }
}
