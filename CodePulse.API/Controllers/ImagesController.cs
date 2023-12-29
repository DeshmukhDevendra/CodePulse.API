﻿using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository imageRepository;
        public ImagesController(IImageRepository imageRepository)
        {
            this.imageRepository = imageRepository;
        }

        //POST: {apiBaseUrl}/api/Images
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file, [FromForm] string fileName, [FromForm] string title) 
        {
            validateFileUpload(file);
            if (ModelState.IsValid) 
            {
                //File upload
                var blogImage = new BlogImage 
                {
                    FileExtension = Path.GetExtension(file.FileName).ToLower(),
                    FileName = fileName,
                    Title = title,
                    DateCreated = DateTime.Now,
                };

                blogImage=await imageRepository.Upload(file, blogImage);

                //Convert Domain Model to Dto
                var response = new BlogImageDto 
                {
                    Id = blogImage.Id,
                    Title = blogImage.Title,
                    DateCreated= blogImage.DateCreated,
                    FileName = blogImage.FileName,
                    FileExtension = blogImage.FileExtension,
                    Url = blogImage.Url
                };
                return Ok(response);
            }
            return BadRequest(ModelState);
        }

        private void validateFileUpload(IFormFile file)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png", ".gif" };
            if (!allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower())) 
            {
                ModelState.AddModelError("file", "Unsupported file format");
            }
            if (file.Length > 10485760) 
            {
                ModelState.AddModelError("file", "File size cannot be more than 10MB");
            }
        }
    }
}