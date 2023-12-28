using CodePulse.API.Models.Domain;

namespace CodePulse.API.Repositories.Interface
{
    public interface IBlogPostRepository
    {
        Task<BlogPost> CreateAsync(BlogPost blogPost);
        Task<IEnumerable<BlogPost>> GetAllBlogPosts();
        Task<BlogPost?> GetBlogPostById(Guid id);
        Task<BlogPost?> UpdateAsync(BlogPost blogPost);
        Task <BlogPost?> DeleteAsync(Guid id);
    }
}
