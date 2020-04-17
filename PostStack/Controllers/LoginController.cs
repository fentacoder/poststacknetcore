using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PostStack.Models;
using PostStack.Utils;

namespace PostStack.Controllers
{
    public class LoginController : Controller
    {

        private HttpClass _httpClass;
        private IConfiguration _configuration;
        private UserValidation _user = new UserValidation();

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClass = new HttpClass(configuration);
        }

        public IActionResult LoginUser()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginUser([Bind("Email,Password")] UserValidation user)
        {
            if (ModelState.IsValid)
            {
                _user.Email = user.Email;
                _user.Password = user.Password;

                var json = JsonSerializer.Serialize(user);

                using(HttpResponseMessage response = await _httpClass.ApiClient.PostAsJsonAsync("api/user/login", json))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string tempStr = await response.Content.ReadAsStringAsync();
                        _user.Id = int.Parse(tempStr);

                        return RedirectToAction("Index", "Home", new { userId = _user.Id });
                    }
                    else
                    {
                        Console.WriteLine(response.ReasonPhrase);
                        return RedirectToAction("LoginUser");
                    }
                }
            }
            else
            {
                user.Password = "";
                return View(user);
            }
        }
    }
}
