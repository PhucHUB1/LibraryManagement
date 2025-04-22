using LibraryManagement.Data;
using LibraryManagement.Models;
using LibraryManagement.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using LibraryManagement.Services.Implement;

namespace LibraryManagement.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;

        public AuthorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<AuthorViewModel> GetAllAuthors()
        {
            return _context.Authors
                .Include(a => a.Books)
                .Select(a => new AuthorViewModel
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Biography = a.Biography,
                    BookCount = a.Books.Count
                })
                .ToList();
        }

        public AuthorViewModel GetAuthorById(int id)
        {
            return _context.Authors
                .Include(a => a.Books)
                .Where(a => a.Id == id)
                .Select(a => new AuthorViewModel
                {
                    Id = a.Id,
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Biography = a.Biography,
                    BookCount = a.Books.Count
                })
                .FirstOrDefault();
        }

        public void AddAuthor(AuthorViewModel model)
        {
            var author = new Author
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Biography = model.Biography
            };

            _context.Authors.Add(author);
            _context.SaveChanges();
        }

        public void UpdateAuthor(AuthorViewModel model)
        {
            var author = _context.Authors.Find(model.Id);
            if (author != null)
            {
                author.FirstName = model.FirstName;
                author.LastName = model.LastName;
                author.Biography = model.Biography;

                _context.Update(author);
                _context.SaveChanges();
            }
        }

        public void DeleteAuthor(int id)
        {
            var author = _context.Authors.Find(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                _context.SaveChanges();
            }
        }
    }
}