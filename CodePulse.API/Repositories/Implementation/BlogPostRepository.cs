﻿using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Repositories.Implementation
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly ApplicationDbContext dbContext;
        public BlogPostRepository(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<BlogPost> CreateAsync(BlogPost blogPost)
        {
            await dbContext.BlogPosts.AddAsync(blogPost);
            await dbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var existingBlogpost = await dbContext.BlogPosts.FirstOrDefaultAsync(x => x.Id == id);
            if (existingBlogpost != null)
            {
                dbContext.BlogPosts.Remove(existingBlogpost);
                await dbContext.SaveChangesAsync();
                return existingBlogpost;
            }
            return null;

        }

        public async Task<IEnumerable<BlogPost>> GetAllBlogPosts()
        {
            return await dbContext.BlogPosts.Include(x => x.Categories).ToListAsync();
        }

        public async Task<BlogPost?> GetBlogPostById(Guid id)
        {
            return await dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<BlogPost?> GetBlogPostByUrlHandle(string urlHandle)
        {
            return await dbContext.BlogPosts.Include(x => x.Categories).FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);
        }

        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var existingBlogPost = await dbContext.BlogPosts.Include(x => x.Categories)
                .FirstOrDefaultAsync(x => x.Id == blogPost.Id);
            
            if(existingBlogPost is null) 
            {
                return null;
            }

            //Update Categories
            dbContext.Entry(existingBlogPost).CurrentValues.SetValues(blogPost);
        
            //Update Categories
            existingBlogPost.Categories = blogPost.Categories;
            await dbContext.SaveChangesAsync();

            return blogPost;
        }
    }
}
