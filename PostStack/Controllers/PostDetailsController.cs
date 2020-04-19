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
        private IHomePageHelper _homePageHelper;

        public PostDetailsController(IConfiguration configuration, IHomePageHelper homePageHelper)
        {
            _configuration = configuration;
            _httpClass = new HttpClass(configuration);
            _homePageHelper = homePageHelper;
        }

        public IActionResult Index(int id,int userId, string title, string body, DateTime createdAt)
        {

            
            if (userId != -1)
            {
                _userId = userId;
                _post.Id = id;
                _homePageHelper.SelectedPostId = id;
                _post.UserId = userId;
                _post.Title = title;
                _post.Body = body;
                _post.CreatedAt = createdAt;
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }
            
            

            return View(_post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Title,Body")] PostValidation post)
        {

            if (ModelState.IsValid)
            {
                post.UserId = _homePageHelper.UserId;
                post.Id = _homePageHelper.SelectedPostId;

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
                        return RedirectToAction("Index", "PostDetails", new
                        {
                            id = post.Id,
                            userId = _homePageHelper.UserId,
                            title = post.Title,
                            body = post.Body,
                            createdAt = post.CreatedAt
                        });
                    }
                }
            }
            else
            {
                return View(post);
            }

        }
    }
}
