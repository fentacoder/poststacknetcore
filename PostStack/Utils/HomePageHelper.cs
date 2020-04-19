using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PostStack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PostStack.Utils
{
    public class HomePageHelper : IHomePageHelper
    {
        public int PostCount { get; set; } = 0;
        public int PageCount { get; set; } = 1;
        public int PageIndex { get; set; } = 1;
        public int UserId { get; set; } = -1;

        public int SelectedPostId { get; set; } = 0;

        public bool PostsLoaded { get; set; } = false;

        public List<PostValidation> PostList { get; set; } = new List<PostValidation>();

        public Dictionary<int, List<PostValidation>> PaginationMap { get; set; } = new Dictionary<int, List<PostValidation>>();

        private HttpClass _httpClass;

        public HomePageHelper(IConfiguration configuration)
        {
            _httpClass = new HttpClass(configuration);
        }

        public async Task<bool> InitializePosts(List<PostValidation> initialList = null)
        {
            if (initialList == null)
            {
                PaginationMap.Clear();

                //grab list of posts from the api

                using (HttpResponseMessage response = await _httpClass.ApiClient.GetAsync($"api/posts/retrieveposts/{UserId}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string tempStr = await response.Content.ReadAsStringAsync();


                        //newtonsoft json was used here ot deserialize the list
                        List<PostValidation> tempList = JsonConvert.DeserializeObject<List<PostValidation>>(tempStr);

                        if (tempList.Count() > 0)
                        {
                            PostCount = tempList.Count();
                            PostList = tempList;

                            //puts values in the dictionary for pagination

                            int mapCounter = 1;
                            int tempCounter = 1;
                            List<PostValidation> placeHolderList = new List<PostValidation>();

                            for (int i = 0; i < tempList.Count(); i++)
                            {
                                if (tempCounter == 11)
                                {
                                    PaginationMap.Add(mapCounter, placeHolderList);
                                    mapCounter++;
                                    tempCounter = 1;
                                    placeHolderList.Clear();
                                }
                                else if (i == tempList.Count() - 1)
                                {
                                    PaginationMap.Add(mapCounter, placeHolderList);
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
                            PageCount = mapCounter;
                            FormatPostList();
                        }
                        return true;
                    }
                    else
                    {

                        Console.WriteLine(response.ReasonPhrase);
                        return true;
                    }
                }
            }
            else
            {
                if (initialList.Count() > 0)
                {
                    PaginationMap.Clear();
                    PostCount = initialList.Count();

                    //puts values in the dictionary for pagination

                    int mapCounter = 1;
                    int tempCounter = 1;
                    List<PostValidation> placeHolderList = new List<PostValidation>();

                    for (int i = 0; i < initialList.Count(); i++)
                    {
                        if (tempCounter == 11)
                        {
                            PaginationMap.Add(mapCounter, placeHolderList);
                            mapCounter++;
                            tempCounter = 1;
                            placeHolderList.Clear();
                        }
                        else if (i == initialList.Count() - 1)
                        {
                            PaginationMap.Add(mapCounter, placeHolderList);
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
                    PageCount = mapCounter;
                    FormatPostList();
                }
                return true;
            }
        }

        public void FormatPostList()
        {
            List<PostValidation> tempList = new List<PostValidation>();

            foreach (var post in PostList)
            {
                post.IdxInList = PageIndex;
                post.PageCount = PageCount;
                tempList.Add(post);
            }

            PostList = tempList;
        }
    }
}
