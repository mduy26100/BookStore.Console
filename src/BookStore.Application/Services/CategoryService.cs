using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Application.Interfaces.SeedWorks;
using BookStore.Application.Interfaces.Services;
using BookStore.Domain.Entities;

namespace BookStore.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task AddCategory(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto));

            if (string.IsNullOrWhiteSpace(categoryDto.Name))
                throw new ArgumentException("Category name cannot be empty");

            try
            {
                // Check if category with the same name already exists
                bool exists = await _unitOfWork.CategoryRepository.CategoryExists(categoryDto.Name);
                if (exists)
                {
                    throw new Exception($"Category with name '{categoryDto.Name}' already exists");
                }

                await _unitOfWork.CategoryRepository.AddAsync(_mapper.Map<Category>(categoryDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add category: {ex.Message}", ex);
            }
        }

        public async Task<bool> CategoryExists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name cannot be empty");

            try
            {
                return await _unitOfWork.CategoryRepository.CategoryExists(name);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check if category exists: {ex.Message}", ex);
            }
        }

        public async Task DeleteCategory(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid category ID");

            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    throw new Exception($"Category with ID {id} not found");
                }

                await _unitOfWork.CategoryRepository.DeleteAsync(category);
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete category: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategory()
        {
            try
            {
                var list = await _unitOfWork.CategoryRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<CategoryDto>>(list);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve categories: {ex.Message}", ex);
            }
        }

        public async Task<CategoryDto> GetCategoryById(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid category ID");

            try
            {
                var category = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (category == null)
                {
                    throw new Exception($"Category with ID {id} not found");
                }

                return _mapper.Map<CategoryDto>(category);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve category: {ex.Message}", ex);
            }
        }

        public async Task UpdateDescriptionCategory(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto));

            if (categoryDto.CategoryID <= 0)
                throw new ArgumentException("Invalid category ID");

            try
            {
                await _unitOfWork.CategoryRepository.UpdateDescription(_mapper.Map<Category>(categoryDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update category description: {ex.Message}", ex);
            }
        }

        public async Task UpdateNameCategory(CategoryDto categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto));

            if (categoryDto.CategoryID <= 0)
                throw new ArgumentException("Invalid category ID");

            if (string.IsNullOrWhiteSpace(categoryDto.Name))
                throw new ArgumentException("Category name cannot be empty");

            try
            {
                await _unitOfWork.CategoryRepository.UpdateName(_mapper.Map<Category>(categoryDto));
                await _unitOfWork.SaveChange();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update category name: {ex.Message}", ex);
            }
        }
    }
}
