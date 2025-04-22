using LibraryManagement.Models.ViewModels.Book;
using LibraryManagement.Services.Implement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LibraryManagement.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthorService _authorService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public BooksController(
            IBookService bookService,
            ICategoryService categoryService,
            IAuthorService authorService,
            IWebHostEnvironment webHostEnvironment)
        {
            _bookService = bookService;
            _categoryService = categoryService;
            _authorService = authorService;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var books = _bookService.GetAllBooks();
            return View(books);
        }

        public IActionResult Details(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Create()
        {
            ViewBag.Categories = _categoryService.GetAllCategories();
            ViewBag.Authors = _authorService.GetAllAuthors();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Process cover image if provided
                if (model.CoverImage != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "books");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(fileStream);
                    }
                    
                    model.ExistingCoverImageUrl = "/images/books/" + uniqueFileName;
                }
                
                _bookService.AddBook(model);
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Categories = _categoryService.GetAllCategories();
            ViewBag.Authors = _authorService.GetAllAuthors();
            return View(model);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Edit(int id)
        {
            var book = _bookService.GetBookForEdit(id);
            if (book == null)
            {
                return NotFound();
            }
            
            ViewBag.Categories = _categoryService.GetAllCategories();
            ViewBag.Authors = _authorService.GetAllAuthors();
            return View(book);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Process cover image if provided
                if (model.CoverImage != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "books");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.CoverImage.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    
                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.CoverImage.CopyToAsync(fileStream);
                    }
                    
                    // Delete old image if exists
                    if (!string.IsNullOrEmpty(model.ExistingCoverImageUrl))
                    {
                        string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, model.ExistingCoverImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    
                    model.ExistingCoverImageUrl = "/images/books/" + uniqueFileName;
                }
                
                _bookService.UpdateBook(model);
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Categories = _categoryService.GetAllCategories();
            ViewBag.Authors = _authorService.GetAllAuthors();
            return View(model);
        }

        [Authorize(Roles = "Admin,Librarian")]
        public IActionResult Delete(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Librarian")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _bookService.DeleteBook(id);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult Borrow(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null || book.AvailableCopies <= 0)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult BorrowConfirmed(int id)
        {
            try
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                _bookService.BorrowBook(id, userId);
                return RedirectToAction("MyBorrows", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var book = _bookService.GetBookById(id);
                return View("Borrow", book);
            }
        }

        [Authorize]
        public IActionResult Reserve(int id)
        {
            var book = _bookService.GetBookById(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult ReserveConfirmed(int id)
        {
            try
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                _bookService.ReserveBook(id, userId);
                return RedirectToAction("MyReservations", "Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                var book = _bookService.GetBookById(id);
                return View("Reserve", book);
            }
        }
    }
}