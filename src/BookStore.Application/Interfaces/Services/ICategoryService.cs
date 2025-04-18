using BookStore.Application.DTOs;

namespace BookStore.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategory();
        Task<CategoryDto> GetCategoryById(int id);
        Task AddCategory(CategoryDto categoryDto);
        Task UpdateNameCategory(CategoryDto categoryDto);
        Task UpdateDescriptionCategory(CategoryDto categoryDto);
        Task DeleteCategory(int id);
        Task<bool> CategoryExists(string name);
    }
}
