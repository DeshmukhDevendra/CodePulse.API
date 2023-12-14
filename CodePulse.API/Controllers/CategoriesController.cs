using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodePulse.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CreateCategoryRequestDto requestDto) 
        {
            // Map DTO (Data Transfer Object) to Domain Model
            var category = new Category
            {
                Name = requestDto.Name,
                UrlHandle = requestDto.UrlHandle,
            };

            await categoryRepository.CreateAsync(category);

            // Map Domain Model to DTO (Data Transfer Object)
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = requestDto.Name,
                UrlHandle = requestDto.UrlHandle,
            };

            return Ok(response);
        }
    }
}
