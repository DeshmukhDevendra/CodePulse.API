using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Repositories.Interface;

namespace CodePulse.API.Repositories.Implementation
{
    public class ImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ApplicationDbContext dbContext;

        public ImageRepository(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, ApplicationDbContext applicationDbContext) 
        {
            this.webHostEnvironment = webHostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.dbContext = applicationDbContext;
        }
        public async Task<BlogImage> Upload(IFormFile file, BlogImage blogImage)
        {
            //Step 1: Upload the image to API/Images
            var localPath = Path.Combine(webHostEnvironment.ContentRootPath, "Images",$"{blogImage.FileName}{blogImage.FileExtension}");

            using var stream = new FileStream(localPath, FileMode.Create);
            await file.CopyToAsync(stream);

            //Step 2: Update the database
            //https://project.com/images/SomeFilename.jpg
            var httpRequest = httpContextAccessor.HttpContext.Request;
            var urlPath = $"{httpRequest.Scheme}://{httpRequest.Host}{httpRequest.PathBase}/Images/{blogImage.FileName}{blogImage.FileExtension}";

            blogImage.Url = urlPath;

            await dbContext.BlogImages.AddAsync(blogImage);
            await dbContext.SaveChangesAsync();

            return blogImage;
        }
    }
}
