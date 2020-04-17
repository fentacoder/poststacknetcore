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

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PostStack.Controllers
{
    public class PostDetailsController : Controller
    {
        private HttpClass _httpClass;
        private IConfiguration _configuration;
        private PostValidation _post = new PostValidation();
        private int _userId = -1;

        public PostDetailsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClass = new HttpClass(configuration);
        }

        public IActionResult Index(int userId, string title, string body, DateTime createdAt)
        {

            if(_userId != -1)
            {
                return View(_post);
            }
            else
            {
                if (userId != -1)
                {
                    _userId = userId;
                    _post.UserId = userId;
                    _post.Title = title;
                    _post.Body = body;
                    _post.CreatedAt = createdAt;
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

            if (!string.IsNullOrWhiteSpace(post.Title) && !string.IsNullOrWhiteSpace(post.Body))
            {
                post.UserId = _userId;
                post.CreatedAt = _post.CreatedAt;

                var json = JsonSerializer.Serialize(post);

                using (HttpResponseMessage response = await _httpClass.ApiClient.PostAsJsonAsync("api/posts/updatepost", json))
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
