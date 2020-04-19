using PostStack.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PostStack.Utils
{
    public interface IHomePageHelper
    {
        int PageCount { get; set; }
        int PageIndex { get; set; }
        Dictionary<int, List<PostValidation>> PaginationMap { get; set; }
        int PostCount { get; set; }
        List<PostValidation> PostList { get; set; }
        bool PostsLoaded { get; set; }
        int UserId { get; set; }

        int SelectedPostId { get; set; }

        void FormatPostList();
        Task<bool> InitializePosts(List<PostValidation> initialList = null);
    }
}