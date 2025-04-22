using LibraryManagement.Models.ViewModels;
using LibraryManagement.Services.Implement;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using LibraryManagement.Models;
using LibraryManagement.Models.ViewModels.Book;

namespace LibraryManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;

        public HomeController(
            ILogger<HomeController> logger,
            IBookService bookService,
            ICategoryService categoryService,
            IAuthorService authorService)
        {
            _logger = logger;
            _bookService = bookService;
            _categoryService = categoryService;
            _authorService = authorService;
        }

        public IActionResult Index()
        {
            ViewBag.LatestBooks = _bookService.GetLatestBooks(6);
            ViewBag.PopularBooks = _bookService.GetPopularBooks(6);
            ViewBag.Categories = _categoryService.GetAllCategories();
            ViewBag.Authors = _authorService.GetAllAuthors();
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Search(string searchTerm, int? categoryId, int? authorId)
        {
            var model = new BookSearchViewModel
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                AuthorId = authorId,
                Books = _bookService.GetFilteredBooks(searchTerm, categoryId, authorId)
            };

            ViewBag.Categories = _categoryService.GetAllCategories();
            ViewBag.Authors = _authorService.GetAllAuthors();

            return View(model);
        }
    }
}