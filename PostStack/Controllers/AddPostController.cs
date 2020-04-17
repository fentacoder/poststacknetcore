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
    public class AddPostController : Controller
    {
        private HttpClass _httpClass;
        private IConfiguration _configuration;
        private PostValidation _post = new PostValidation();
        private int _userId = -1;

        public AddPostController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClass = new HttpClass(configuration);
        }

        public IActionResult Index(int userId = -1)
        {
            if (_userId != -1)
            {
                return View(_post);
            }
            else
            {
                if (userId != -1)
                {
                    _userId = userId;
                    _post.UserId = userId;
                }
                else
                {
                    return RedirectToAction("LoginUser", "Login");
                }
            }


            return View(_post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Title,Body")] PostValidation post)
        {

            if(!string.IsNullOrWhiteSpace(post.Title) && !string.IsNullOrWhiteSpace(post.Body))
            {

                var json = JsonSerializer.Serialize(post);

                using (HttpResponseMessage response = await _httpClass.ApiClient.PostAsJsonAsync("api/posts/addpost", json))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home", new { userId = _userId });
                    }
                    else
                    {
                        Console.WriteLine(response.ReasonPhrase);
                        return RedirectToAction("Index");
                    }
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

        }
    }
}