using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly IBlogPostRepository blogPostRepository;
        private readonly ICategoryRepository categoryRepository;
        public BlogPostsController(IBlogPostRepository blogPostRepository, ICategoryRepository categoryRepository)
        {
            this.blogPostRepository = blogPostRepository;
            this.categoryRepository = categoryRepository;
        }

        //POST: https://localhost:44380/api/BlogPosts
        [HttpPost]
        public async Task<IActionResult> CreateBlogPosts([FromBody]CreateBlogPostRequestDto requestDto) 
        {
            var blogPost = new BlogPost 
            {
                Title = requestDto.Title,
                Author = requestDto.Author,
                Content = requestDto.Content,
                FeaturedImageUrl = requestDto.FeaturedImageUrl,
                IsVisible = requestDto.IsVisible,
                PublishedDate = requestDto.PublishedDate,
                ShortDescription = requestDto.ShortDescription,
                UrlHandle = requestDto.UrlHandle,
                Categories = new List<Category>()
            };

            foreach (var categoryGuid in requestDto.Categories) 
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if (existingCategory != null) 
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            blogPost = await blogPostRepository.CreateAsync(blogPost);

            //Convert Domain Model back to DTO
            var response = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Author = blogPost.Author,
                Content = blogPost.Content,
                FeaturedImageUrl = blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                ShortDescription = blogPost.ShortDescription,
                UrlHandle = blogPost.UrlHandle,
                Categories = blogPost.Categories.Select(x => new CategoryDto
                {
                    Id =x.Id,
                    Name = x.Name,
                    UrlHandle=x.UrlHandle
                }).ToList()
            };
            return Ok(response);
        }

        //GET: https://localhost:44380/api/BlogPosts
        [HttpGet]
        public async Task<IActionResult> GetAllBlogPosts() 
        {
            var blogPosts = await blogPostRepository.GetAllBlogPosts();
            if (blogPosts is null) 
            {
                return NotFound();
            }

            //Convert Domain to DTO
            var response = new List<BlogPostDto>();
            foreach (var blogPost in blogPosts)
            {
                response.Add(new BlogPostDto 
                {
                    Id = blogPost.Id,
                    Title = blogPost.Title,
                    Author = blogPost.Author,
                    Content = blogPost.Content,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    IsVisible = blogPost.IsVisible,
                    PublishedDate = blogPost.PublishedDate,
                    ShortDescription = blogPost.ShortDescription,
                    UrlHandle = blogPost.UrlHandle,
                    Categories = blogPost.Categories.Select(x => new CategoryDto 
                    {
                        Id=x.Id,
                        Name = x.Name,
                        UrlHandle=x.UrlHandle
                    }).ToList()
                });
            }
            return Ok(response);
        }
    }
}
