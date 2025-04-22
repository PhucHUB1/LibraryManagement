using LibraryManagement.Models.ViewModels;
using LibraryManagement.Services.Implement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Controllers
{
    [Authorize(Roles = "Admin,Librarian")]
    public class AuthorsController : Controller
    {
        private readonly IAuthorService _authorService;

        public AuthorsController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        public IActionResult Index()
        {
            var authors = _authorService.GetAllAuthors();
            return View(authors);
        }

        public IActionResult Details(int id)
        {
            var author = _authorService.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                _authorService.AddAuthor(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var author = _authorService.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AuthorViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _authorService.UpdateAuthor(model);
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var author = _authorService.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _authorService.DeleteAuthor(id);
            return RedirectToAction(nameof(Index));
        }
    }
}