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

        private IHomePageHelper _homePageHelper;

        public AddPostController(IConfiguration configuration, IHomePageHelper homePageHelper)
        {
            _configuration = configuration;
            _httpClass = new HttpClass(configuration);
            _homePageHelper = homePageHelper;
        }

        public IActionResult Index(int userId = -1)
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
            


            return View(_post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([Bind("Title,Body")] PostValidation post)
        {

            if (ModelState.IsValid)
            {
                post.UserId = _homePageHelper.UserId;
                var json = JsonSerializer.Serialize(post);

                using (HttpResponseMessage response = await _httpClass.ApiClient.PostAsJsonAsync("api/posts/addpost", json))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index", "Home", new { userId = _homePageHelper.UserId });
                    }
                    else
                    {
                        Console.WriteLine(response.ReasonPhrase);
                        return RedirectToAction("Index", "AddPost", new { userId = _homePageHelper.UserId });
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