using BookStore.Application.Interfaces.Repositories;
using BookStore.Domain.Entities;
using BookStore.Infrastructure.Data;
using BookStore.Infrastructure.SeedWorks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BookStore.Infrastructure.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> CategoryExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty");

            try
            {
                return await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check if category exists: {ex.Message}", ex);
            }
        }

        public async Task UpdateDescription(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category.CategoryID <= 0)
                throw new ArgumentException("Invalid category ID");

            try
            {
                var entity = await _context.Categories.FindAsync(category.CategoryID);
                if (entity == null)
                {
                    throw new Exception($"Category with ID {category.CategoryID} not found");
                }
                entity.Description = category.Description;
                _context.Categories.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update category description: {ex.Message}", ex);
            }
        }

        public async Task UpdateName(Category category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            if (category.CategoryID <= 0)
                throw new ArgumentException("Invalid category ID");

            if (string.IsNullOrWhiteSpace(category.Name))
                throw new ArgumentException("Category name cannot be empty");

            try
            {
                var entity = await _context.Categories.FindAsync(category.CategoryID);
                if (entity == null)
                {
                    throw new Exception($"Category with ID {category.CategoryID} not found");
                }

                bool nameExists = await _context.Categories
                    .AnyAsync(c => c.Name.ToLower() == category.Name.ToLower() && c.CategoryID != category.CategoryID);

                if (nameExists)
                {
                    throw new Exception($"Category with name '{category.Name}' already exists");
                }

                entity.Name = category.Name;
                _context.Categories.Update(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update category name: {ex.Message}", ex);
            }
        }
    }
}
