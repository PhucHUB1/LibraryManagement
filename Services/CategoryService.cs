using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LibraryManagement.Services.Implement;

namespace LibraryManagement.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<CategoryViewModel> GetAllCategories()
        {
            return _context.Categories
                .Include(c => c.Books)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    BookCount = c.Books.Count
                })
                .ToList();
        }

        public CategoryViewModel GetCategoryById(int id)
        {
            return _context.Categories
                .Include(c => c.Books)
                .Where(c => c.Id == id)
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    BookCount = c.Books.Count
                })
                .FirstOrDefault();
        }

        public void AddCategory(CategoryViewModel model)
        {
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description
            };

            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        public void UpdateCategory(CategoryViewModel model)
        {
            var category = _context.Categories.Find(model.Id);
            if (category != null)
            {
                category.Name = model.Name;
                category.Description = model.Description;

                _context.Update(category);
                _context.SaveChanges();
            }
        }

        public void DeleteCategory(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
            }
        }
    }
}