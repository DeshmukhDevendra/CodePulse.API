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

        //POST: {apibaseurl}/api/BlogPosts
        [HttpPost]
        public async Task<IActionResult> CreateBlogPosts([FromBody] CreateBlogPostRequestDto requestDto)
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
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };
            return Ok(response);
        }

        //GET: {apibaseurl}/api/BlogPosts
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
                        Id = x.Id,
                        Name = x.Name,
                        UrlHandle = x.UrlHandle
                    }).ToList()
                });
            }
            return Ok(response);
        }

        //GET: {apibaseurl}/api/BlogPosts/{{id}}
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetBlogById([FromRoute] Guid id)
        {
            var blogpost = await blogPostRepository.GetBlogPostById(id);
            if (blogpost is null)
            {
                return NotFound();
            }
            //Convert Model to DTO

            var response = new BlogPostDto()
            {
                Id = blogpost.Id,
                Author = blogpost.Author,
                Content = blogpost.Content,
                FeaturedImageUrl = blogpost.FeaturedImageUrl,
                IsVisible = blogpost.IsVisible,
                PublishedDate = blogpost.PublishedDate,
                ShortDescription = blogpost.ShortDescription,
                UrlHandle = blogpost.UrlHandle,
                Title = blogpost.Title,
                Categories = blogpost.Categories.Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    UrlHandle = x.UrlHandle
                }).ToList()
            };
            return Ok(response);
        }

        //PUT: {apibaseurl}/api/BlogPosts/{{id}}
        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateBlogPostById([FromRoute] Guid id, [FromBody] UpdateBlogPostRequestDto requestDto)
        {
            // Convert DTO to Domain Model
            var blogPost = new BlogPost
            {
                Id = id,
                Author = requestDto.Author,
                Content = requestDto.Content,
                FeaturedImageUrl = requestDto.FeaturedImageUrl,
                IsVisible = requestDto.IsVisible,
                PublishedDate = requestDto.PublishedDate,
                UrlHandle = requestDto.UrlHandle,
                Title = requestDto.Title,
                ShortDescription = requestDto.ShortDescription,
                Categories = new List<Category>() { }
            };

            foreach (var categoryGuid in requestDto.Categories)
            {
                var existingCategory = await categoryRepository.GetById(categoryGuid);
                if (existingCategory is not null)
                {
                    blogPost.Categories.Add(existingCategory);
                }
            }

            //Call repository to update BlogPost Domain Model
            var updatedBlogPost = await blogPostRepository.UpdateAsync(blogPost);

            if (updatedBlogPost is null)
            {
                return NotFound();
            }

            //Convert domain model to DTO
            var response = new BlogPostDto
            {
                Id = updatedBlogPost.Id,
                Author = updatedBlogPost.Author,
                Content = updatedBlogPost.Content,
                FeaturedImageUrl = updatedBlogPost.FeaturedImageUrl,
                IsVisible = updatedBlogPost.IsVisible,
                PublishedDate = updatedBlogPost.PublishedDate,
                UrlHandle = updatedBlogPost.UrlHandle,
                Title = updatedBlogPost.Title,
                ShortDescription = updatedBlogPost.ShortDescription,
                Categories = updatedBlogPost.Categories.Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    UrlHandle = c.UrlHandle
                }).ToList()
            };
            return Ok(response);
        }

        //DELETE: {apibaseurl}/api/BlogPosts/{{id}}
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteBlogPost([FromRoute] Guid id) 
        {
            var blogPost = await blogPostRepository.DeleteAsync(id);
            if(blogPost is null) 
            {
                return NotFound(); 
            }
            
            //Prepare the response DTO
            var response = new BlogPostDto
            {
                Id=blogPost.Id,
                Author = blogPost.Author,
                Content = blogPost.Content,
                FeaturedImageUrl=blogPost.FeaturedImageUrl,
                IsVisible = blogPost.IsVisible,
                PublishedDate = blogPost.PublishedDate,
                UrlHandle = blogPost.UrlHandle,
                Title = blogPost.Title,
                ShortDescription = blogPost.ShortDescription
            };
            return Ok(response);
        }
    }
}
