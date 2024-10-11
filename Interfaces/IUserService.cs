using LiberaryManagmentSystem.Dtos;
using Microsoft.AspNetCore.Identity;
using LiberaryManagmentSystem.Models;
using LiberaryManagmentSystem.ViewModels;
using System.Drawing.Printing;

public interface IUserService
{
    Task<IdentityResult> RegisterUser(RegisterDto registerDto);
    Task<IdentityResult> RegisterUserWithRole(RegisterDto registerDto);
    Task<SignInResult> LoginUser(LoginDto loginDto);
    Task LogoutUser();
    Task<IdentityResult> UpdateUserProfile(string userId, UpdateUserDto updateUserDto);
    Task<ApplicationUser> GetUserByIdAsync(string userId);
    Task<IdentityResult> ChangeUserPasswordAsync(string userId, string currentPassword, string newPassword);
    Task<IdentityResult> DeleteUserAsync(string userId, string currentUserId);
    Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(int page, int pageSize);
    Task<int> GetTotalUserCount();
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user, EditUserDto editUserDto);
    Task<IEnumerable<string>> GetAvailableRolesAsync();
    Task<ApplicationUser> FindUserByEmailAsync(string email);
    Task<EditUserDto> GetEditUserViewModelAsync(string userId);
    Task<PagedUsersViewModel> GetPagedUsersAsync(int page,int pageSize);
}
