using CodePulse.API.Data;
using CodePulse.API.Models.Domain;
using CodePulse.API.Models.DTO;
using CodePulse.API.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
        public async Task<IActionResult> CreateCategory([FromBody]CreateCategoryRequestDto requestDto) 
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

        //GET: https://localhost:44380/api/Categories
        [HttpGet]
        [Authorize(Roles ="Reader")]
        public async Task<IActionResult> GetAllCategories() 
        {
            var category = await categoryRepository.GetAllAsync();
            
            //Map Domain model to DTO
            var response = new List<CategoryDto>();
            foreach (var item in category) 
            {
                response.Add(new CategoryDto
                {
                    Id = item.Id,
                    Name = item.Name,   
                    UrlHandle = item.UrlHandle,           
                });
            }
            
            return Ok(response);
        }

        //GET: https://localhost:44380/api/Categories/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetCategoryById([FromRoute]Guid id) 
        {
            var existingCategory = await categoryRepository.GetById(id);
            if (existingCategory is null)
            {
                return NotFound();
            }

            var response = new CategoryDto 
            {
                Id = existingCategory.Id, 
                Name = existingCategory.Name, 
                UrlHandle = existingCategory.UrlHandle
            }; 

            return Ok(response);
        }

        //PUT: https://localhost:44380/api/Categories/{id}
        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> EditCategory([FromRoute] Guid id, [FromBody] UpdateCategoryRequestDto requestDto) 
        {
            //Convert to DTO to Domain Model

            var category = new Category 
            {     
                Id = id,
                Name = requestDto.Name,
                UrlHandle = requestDto.UrlHandle,
            };

            category = await categoryRepository.UpdateAsync(category);
            if(category is null) 
            {
                return NotFound();
            }

            //Convert Domain Model to DTO
            var response = new CategoryDto
            {
                Id=category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };

            return Ok(response);
        }

        //DELETE: https://localhost:44380/api/Categories/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] Guid id) 
        {
            var category = await categoryRepository.DeleteAsync(id);

            if (category is null) 
            {
                return NotFound();
            }

            //Convert Domain Model to DTO
            var response = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                UrlHandle = category.UrlHandle
            };
            return Ok(response);
        }
    }
}
