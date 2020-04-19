using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PostStack.Models;
using PostStack.Utils;

namespace PostStack.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private HttpClass _httpClass;
        private IConfiguration _configuration;
        private IHomePageHelper _homePageHelper;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration, IHomePageHelper homePageHelper)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClass = new HttpClass(configuration);
            _homePageHelper = homePageHelper;
        }

        public async Task<IActionResult> Index(int userId = -1)
        {
            //if the user came from another controller
            if(userId != -1)
            {
                _homePageHelper.UserId = userId;
            }

            //if user is logged in
            if (_homePageHelper.UserId != -1)
            {
                if (!_homePageHelper.PostsLoaded)
                {
                    bool success = await _homePageHelper.InitializePosts();
                }
                else
                {
                    _homePageHelper.PostsLoaded = false;
                }
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }

            return View(_homePageHelper.PostList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> DeletePost(int deleteIdx = -1)
        {
            //the user deleted a post
            if(deleteIdx != -1)
            {
                
                var values = new Dictionary<string, string>
                {
                    {"UserId", _homePageHelper.UserId.ToString() },
                    {"PostId", _homePageHelper.PostList[deleteIdx].Id.ToString() }
                };

                _homePageHelper.PostList.RemoveAt(deleteIdx);
                bool success = await _homePageHelper.InitializePosts(_homePageHelper.PostList);
                _homePageHelper.PostsLoaded = true;

                var json = JsonSerializer.Serialize(values);

                using(HttpResponseMessage response = await _httpClass.ApiClient.PostAsJsonAsync("api/posts/deletepost", json))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        Console.WriteLine(response.ReasonPhrase);
                        return RedirectToAction("Index");
                    }
                }
            }
            return RedirectToAction("LoginUser", "Login");
        }

        public IActionResult ChangePage(int i = 0)
        {
            //sets the index that the user wants to go to
            if(i != 0)
            {
                _homePageHelper.PostList = _homePageHelper.PaginationMap[i];
                _homePageHelper.PageIndex = i;

                _homePageHelper.FormatPostList();
            }

            return RedirectToAction("Index");
        }

        public IActionResult LogUserOut()
        {
            return RedirectToAction("LoginUser", "Login");
        }

        public IActionResult RedirectToAdd()
        {
            Console.WriteLine(_homePageHelper.UserId);

            return RedirectToAction("Index", "AddPost", new { userId = _homePageHelper.UserId });
        }

        public IActionResult RedirectToDetails(int deleteIdx)
        {
            PostValidation newPost = _homePageHelper.PostList[deleteIdx];

            return RedirectToAction("Index", "PostDetails", new
            {
                id = newPost.Id,
                userId = _homePageHelper.UserId,
                title = newPost.Title,
                body = newPost.Body,
                createdAt = newPost.CreatedAt
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
