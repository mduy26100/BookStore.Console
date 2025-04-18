using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Domain.Entities;

namespace BookStore.Application.Interfaces.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task UpdateName(Category category);
        Task UpdateDescription(Category category);
        Task<bool> CategoryExists(string name);
    }
}
