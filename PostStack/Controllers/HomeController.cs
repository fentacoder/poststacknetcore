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

        private int _postCount = 0;
        private int _pageCount = 0;
        private int _pageIndex = 1;
        private bool _postsLoaded = false;

        private List<PostValidation> _postList;
        private Dictionary<int, List<PostValidation>> _paginationMap = new Dictionary<int, List<PostValidation>>();
        private PostValidation _post = new PostValidation();
        private UserValidation _user = new UserValidation();
        private HttpClass _httpClass;
        private IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger,IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClass = new HttpClass(configuration);
        }

        public IActionResult Index(int userId = -1)
        {
            //if the user came from another controller
            if(userId != -1)
            {
                _user.Id = userId;
            }

            //if user is logged in
            if(_user.Id != -1)
            {
                if (!_postsLoaded)
                {
                    InitializePosts();
                }
                else
                {
                    _postsLoaded = false;
                }
            }
            else
            {
                return RedirectToAction("LoginUser", "Login");
            }

            return View(_postList);
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
                _postList.RemoveAt(deleteIdx);
                InitializePosts(_postList);
                _postsLoaded = true;

                var values = new Dictionary<string, string>
                {
                    {"UserId", _user.Id.ToString() },
                    {"CreatedAt", _postList[deleteIdx].CreatedAt.ToString() }
                };

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
                _postList = _paginationMap[i];
                _pageIndex = i;

                FormatPostList();
            }

            return RedirectToAction("Index");
        }

        public IActionResult LogUserOut()
        {
            return RedirectToAction("LoginUser", "Login");
        }

        public IActionResult RedirectToAdd()
        {
            return RedirectToAction("Index", "AddPost", new { userId = _user.Id });
        }

        public IActionResult RedirectToDetails(int deleteIdx)
        {
            PostValidation newPost = _postList[deleteIdx];

            return RedirectToAction("Index", "PostDetails", new
            {
                userId = _user.Id,
                title = newPost.Title,
                body = newPost.Body,
                createdAt = newPost.CreatedAt
            });
        }

        private async void InitializePosts(List<PostValidation> initialList = null)
        {
            if (initialList == null)
            {
                _paginationMap.Clear();

                //grab list of posts from the api

                using (HttpResponseMessage response = await _httpClass.ApiClient.GetAsync($"api/posts/{_user.Id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string tempStr = await response.Content.ReadAsStringAsync();

                        List<PostValidation> tempList = JsonSerializer.Deserialize<List<PostValidation>>(tempStr);

                        if (tempList.Count() > 0)
                        {
                            _postCount = tempList.Count();
                            _postList = tempList;

                            //puts values in the dictionary for pagination

                            int mapCounter = 1;
                            int tempCounter = 1;
                            List<PostValidation> placeHolderList = new List<PostValidation>();

                            for (int i = 0; i < tempList.Count(); i++)
                            {
                                if (tempCounter == 11)
                                {
                                    _paginationMap.Add(mapCounter, placeHolderList);
                                    mapCounter++;
                                    tempCounter = 1;
                                    placeHolderList.Clear();
                                }
                                else if (i == tempList.Count() - 1)
                                {
                                    _paginationMap.Add(mapCounter, placeHolderList);
                                    tempCounter = 1;
                                    placeHolderList.Clear();
                                }
                                else
                                {
                                    placeHolderList.Add(tempList[i]);
                                    tempCounter++;
                                }
                            }

                            //set the pagecount
                            _pageCount = mapCounter;
                        }
                    }
                    else
                    {

                        Console.WriteLine(response.ReasonPhrase);

                    }
                }
            }
            else
            {
                if (initialList.Count() > 0)
                {
                    _paginationMap.Clear();
                    _postCount = initialList.Count();

                    //puts values in the dictionary for pagination

                    int mapCounter = 1;
                    int tempCounter = 1;
                    List<PostValidation> placeHolderList = new List<PostValidation>();

                    for (int i = 0; i < initialList.Count(); i++)
                    {
                        if (tempCounter == 11)
                        {
                            _paginationMap.Add(mapCounter, placeHolderList);
                            mapCounter++;
                            tempCounter = 1;
                            placeHolderList.Clear();
                        }
                        else if (i == initialList.Count() - 1)
                        {
                            _paginationMap.Add(mapCounter, placeHolderList);
                            tempCounter = 1;
                            placeHolderList.Clear();
                        }
                        else
                        {
                            placeHolderList.Add(initialList[i]);
                            tempCounter++;
                        }
                    }

                    //set the pagecount
                    _pageCount = mapCounter;
                }
            }
        }

        private void FormatPostList()
        {
            List<PostValidation> tempList = new List<PostValidation>();

            foreach(var post in _postList)
            {
                post.IdxInList = _pageIndex;
                post.PageCount = _pageCount;
                tempList.Add(post);
            }

            _postList = tempList;
        }









        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
