using LibraryManagement.Models.ViewModels;

namespace LibraryManagement.Services.Implement
{
    public interface IAuthorService
    {
        List<AuthorViewModel> GetAllAuthors();
        AuthorViewModel GetAuthorById(int id);
        void AddAuthor(AuthorViewModel model);
        void UpdateAuthor(AuthorViewModel model);
        void DeleteAuthor(int id);
    }
}