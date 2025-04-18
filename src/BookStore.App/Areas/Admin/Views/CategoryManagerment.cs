using BookStore.App.Common;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.Services;
using System;
using System.Threading.Tasks;

namespace BookStore.App.Areas.Admin.Views
{
    public class CategoryManagement
    {
        private readonly ICategoryService _categoryService;

        public CategoryManagement(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task ManageCategories()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Category Management ===");
                Console.WriteLine("1. View All Categories");
                Console.WriteLine("2. Add New Category");
                Console.WriteLine("3. Edit Category");
                Console.WriteLine("4. Delete Category");
                Console.WriteLine("0. Back to Admin Dashboard");

                string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2", "3", "4" });

                if (choice == null)
                    continue;

                try
                {
                    switch (choice)
                    {
                        case "1":
                            await ViewAllCategories();
                            break;
                        case "2":
                            await AddCategory();
                            break;
                        case "3":
                            await EditCategory();
                            break;
                        case "4":
                            await DeleteCategory();
                            break;
                        case "0":
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private async Task ViewAllCategories()
        {
            Console.Clear();
            Console.WriteLine("=== All Categories ===");

            var categories = await _categoryService.GetAllCategory();

            if (categories == null || !categories.Any())
            {
                Console.WriteLine("No categories found.");
                return;
            }

            Console.WriteLine("ID\tName\t\tDescription");
            Console.WriteLine("--------------------------------------------------");

            foreach (var category in categories)
            {
                Console.WriteLine($"{category.CategoryID}\t{category.Name}\t\t{category.Description}");
            }
        }

        private async Task AddCategory()
        {
            Console.Clear();
            Console.WriteLine("=== Add New Category ===");

            string name = await GetUniqueCategoryName("Enter category name: ");
            if (name == null) return;

            string description = InputValidator.GetValidString("Enter category description (optional): ",
                input => true,
                "Invalid description format.");
            if (description == null) return;

            var categoryDto = new CategoryDto
            {
                Name = name,
                Description = description
            };

            try
            {
                await _categoryService.AddCategory(categoryDto);
                Console.WriteLine("Category added successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding category: {ex.Message}");
            }
        }

        private async Task EditCategory()
        {
            Console.Clear();
            Console.WriteLine("=== Edit Category ===");

            await ViewAllCategories();

            int? categoryId = InputValidator.GetValidInteger("Enter category ID to edit: ",
                id => id > 0,
                "Invalid category ID. Please enter a positive number.");
            if (categoryId == null) return;

            try
            {
                var category = await _categoryService.GetCategoryById(categoryId.Value);
                if (category == null)
                {
                    Console.WriteLine($"Category with ID {categoryId} not found.");
                    return;
                }

                Console.WriteLine($"Editing Category: {category.Name}");
                Console.WriteLine("1. Edit Name");
                Console.WriteLine("2. Edit Description");
                Console.WriteLine("0. Cancel");

                string choice = InputValidator.GetValidMenuChoice("Choose option: ", new[] { "0", "1", "2" });
                if (choice == null) return;

                switch (choice)
                {
                    case "1":
                        await EditCategoryName(category);
                        break;
                    case "2":
                        await EditCategoryDescription(category);
                        break;
                    case "0":
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task EditCategoryName(CategoryDto category)
        {
            string newName = await GetUniqueCategoryNameForEdit("Enter new category name: ", category.Name);
            if (newName == null) return;

            category.Name = newName;

            try
            {
                await _categoryService.UpdateNameCategory(category);
                Console.WriteLine("Category name updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating category name: {ex.Message}");
            }
        }

        private async Task EditCategoryDescription(CategoryDto category)
        {
            string newDescription = InputValidator.GetValidString("Enter new category description (optional): ",
                input => true,
                "Invalid description format.");
            if (newDescription == null) return;

            category.Description = newDescription;

            try
            {
                await _categoryService.UpdateDescriptionCategory(category);
                Console.WriteLine("Category description updated successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating category description: {ex.Message}");
            }
        }

        private async Task DeleteCategory()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Category ===");

            await ViewAllCategories();

            int? categoryId = InputValidator.GetValidInteger("Enter category ID to delete: ",
                id => id > 0,
                "Invalid category ID. Please enter a positive number.");
            if (categoryId == null) return;

            try
            {
                var category = await _categoryService.GetCategoryById(categoryId.Value);
                if (category == null)
                {
                    Console.WriteLine($"Category with ID {categoryId} not found.");
                    return;
                }

                Console.WriteLine($"Are you sure you want to delete the category '{category.Name}'?");
                string confirmation = InputValidator.GetValidMenuChoice("Enter 'Y' to confirm or 'N' to cancel: ", new[] { "Y", "N", "y", "n" });
                if (confirmation == null) return;

                if (confirmation.ToUpper() == "Y")
                {
                    await _categoryService.DeleteCategory(categoryId.Value);
                    Console.WriteLine("Category deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Deletion cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private async Task<string> GetUniqueCategoryName(string prompt)
        {
            int attempts = 0;
            const int maxRetries = 3;

            while (attempts < maxRetries)
            {
                string name = InputValidator.GetNonEmptyString(prompt);
                if (name == null) return null;

                try
                {
                    bool exists = await _categoryService.CategoryExists(name);
                    if (!exists)
                    {
                        return name;
                    }

                    attempts++;
                    Console.WriteLine($"Category with name '{name}' already exists. Please choose a different name.");
                    Console.WriteLine($"Attempts remaining: {maxRetries - attempts}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking category name: {ex.Message}");
                    return null;
                }
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }

        private async Task<string> GetUniqueCategoryNameForEdit(string prompt, string currentName)
        {
            int attempts = 0;
            const int maxRetries = 3;

            while (attempts < maxRetries)
            {
                string name = InputValidator.GetNonEmptyString(prompt);
                if (name == null) return null;

                if (name.Equals(currentName, StringComparison.OrdinalIgnoreCase))
                {
                    return name;
                }

                try
                {
                    bool exists = await _categoryService.CategoryExists(name);
                    if (!exists)
                    {
                        return name;
                    }

                    attempts++;
                    Console.WriteLine($"Category with name '{name}' already exists. Please choose a different name.");
                    Console.WriteLine($"Attempts remaining: {maxRetries - attempts}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking category name: {ex.Message}");
                    return null;
                }
            }

            Console.WriteLine("Maximum retry attempts exceeded. Returning to previous menu.");
            return null;
        }
    }
}
