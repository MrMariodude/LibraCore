using LiberaryManagmentSystem.Models;

namespace LiberaryManagmentSystem.ViewModels
{
    public class PagedUsersViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
        public int CurrentPage { get; set; }  
        public int PageSize { get; set; } 
        public int TotalUsers { get; set; }  
    }

}
