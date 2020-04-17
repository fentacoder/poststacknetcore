using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PostStackDataAccessLibrary.DbModels;

namespace PostStackApi.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        IConfiguration _configuration;

        public PostsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("retrieveposts/{userId}")]
        public List<Post> RetrievePosts(int userId)
        {

            var post = new Post();

            List<Post> tempList = post.GetPosts(_configuration.GetConnectionString("DefaultConnection"), userId);

            return tempList;
        }

        [HttpPost]
        [Route("deletepost")]
        public ActionResult<bool> DeletePost([FromBody] string jsonObj)
        {
            Dictionary<string, string> tempPost = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonObj);

            Post post = new Post();

            int userId = int.Parse(tempPost["UserId"]);

            DateTime createdAt = DateTime.Parse(tempPost["CreatedAt"]);

            bool result = post.DeletePost(_configuration.GetConnectionString("DefaultConnection"), userId, createdAt);

            if (!result)
            {
                return false;
            }

            return true;
        }

        [HttpPost]
        [Route("addpost")]
        public ActionResult<bool> AddPost([FromBody] string jsonObj)
        {

            Post tempPost = JsonSerializer.Deserialize<Post>(jsonObj);

            Post post = new Post();

            bool success = post.AddPost(_configuration.GetConnectionString("DefaultConnection"), tempPost.UserId, tempPost.Title, tempPost.Body);

            if (!success)
            {
                return false;
            }

            return true;
        }


        [HttpPost]
        [Route("updatepost")]
        public ActionResult<bool> UpdatePost([FromBody] string jsonObj)
        {

            Post tempPost = JsonSerializer.Deserialize<Post>(jsonObj);

            Post post = new Post();

            bool success = post.UpdatePost(_configuration.GetConnectionString("DefaultConnection"),
                tempPost.UserId, tempPost.CreatedAt, tempPost.Title, tempPost.Body);

            if (!success)
            {
                return false;
            }

            return true;
        }

    }
}