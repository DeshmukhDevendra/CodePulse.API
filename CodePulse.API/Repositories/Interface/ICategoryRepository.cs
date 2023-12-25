using CodePulse.API.Models.Domain;

namespace CodePulse.API.Repositories.Interface
{
    public interface ICategoryRepository
    {
        Task<Category> CreateAsync(Category category);
        Task<Category?> UpdateAsync(Category category);
        Task <Category?> DeleteAsync(Guid id);
        Task<Category?> GetById(Guid id);
        Task<IEnumerable<Category>> GetAllAsync();
    }
}
