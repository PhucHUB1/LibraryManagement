using LibraryManagement.Models.ViewModels;

namespace LibraryManagement.Services.Implement
{
    public interface ICategoryService
    {
        List<CategoryViewModel> GetAllCategories();
        CategoryViewModel GetCategoryById(int id);
        void AddCategory(CategoryViewModel model);
        void UpdateCategory(CategoryViewModel model);
        void DeleteCategory(int id);
    }
}