using LiberaryManagmentSystem.Dtos;
using LiberaryManagmentSystem.Models;
using LiberaryManagmentSystem.Repository;
using LiberaryManagmentSystem.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace LiberaryManagmentSystem.Services
{
    /// <summary>
    /// Service for managing user-related operations.
    /// </summary>
    public class UserService(AccountRepository accountRepository, UserManager<ApplicationUser> userManager) : IUserService
    {
        /// <summary>
        /// Registers a new user and assigns a role.
        /// </summary>
        /// <param name="registerDto">The registration details of the user.</param>
        /// <returns>
        /// An <see cref="IdentityResult"/> indicating the success or failure of the registration.
        /// </returns>
        public async Task<IdentityResult> RegisterUserWithRole(RegisterDto registerDto)
        {
            // 1) Check if email exists
            var existingUser = await accountRepository.FindUserByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                var identityError = new IdentityError { Description = "Email is already in use." };
                return IdentityResult.Failed(identityError);
            }

            // 2) Register the user
            var result = await accountRepository.RegisterUser(registerDto);
            if (!result.Succeeded)
            {
                return result;
            }

            // 3) Assign the role to the user
            var user = await accountRepository.FindUserByEmailAsync(registerDto.Email);
            var roleResult = await userManager.AddToRoleAsync(user, registerDto.Role);

            return roleResult.Succeeded ? IdentityResult.Success : roleResult;
        }

        /// <summary>
        /// Registers a new user without assigning a role.
        /// </summary>
        /// <param name="registerDto">The registration details of the user.</param>
        /// <returns>
        /// An <see cref="IdentityResult"/> indicating the success or failure of the registration.
        /// </returns>
        public async Task<IdentityResult> RegisterUser(RegisterDto registerDto)
        {
            return await accountRepository.RegisterUser(registerDto);
        }

        /// <summary>
        /// Logs in a user based on the provided login details.
        /// </summary>
        /// <param name="loginDto">The login details of the user.</param>
        /// <returns>
        /// A <see cref="SignInResult"/> indicating the success or failure of the login attempt.
        /// </returns>
        public async Task<SignInResult> LoginUser(LoginDto loginDto)
        {
            return await accountRepository.LoginUser(loginDto);
        }

        /// <summary>
        /// Logs out the currently logged-in user.
        /// </summary>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public async Task LogoutUser()
        {
            await accountRepository.LogoutUser();
        }

        /// <summary>
        /// Updates the user profile for a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="updateUserDto">The new user profile data.</param>
        /// <returns>
        /// An <see cref="IdentityResult"/> indicating the success or failure of the update.
        /// </returns>
        public async Task<IdentityResult> UpdateUserProfile(string userId, UpdateUserDto updateUserDto)
        {
            return await accountRepository.UpdateUserProfile(userId, updateUserDto);
        }

        /// <summary>
        /// Retrieves a user by their ID asynchronously.
        /// </summary>
        /// <param name="userId">The ID of the user to retrieve.</param>
        /// <returns>
        /// The <see cref="ApplicationUser"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<ApplicationUser> GetUserByIdAsync(string userId)
        {
            return await accountRepository.GetUserByIdAsync(userId);
        }

        /// <summary>
        /// Changes the password for a specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="currentPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>
        /// An <see cref="IdentityResult"/> indicating the success or failure of the password change.
        /// </returns>
        public async Task<IdentityResult> ChangeUserPasswordAsync(string userId, string currentPassword, string newPassword)
        {
            return await accountRepository.ChangeUserPasswordAsync(userId, currentPassword, newPassword);
        }

        /// <summary>
        /// Deletes a user by their ID, with authorization checks against the current user.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <param name="currentUserId">The ID of the currently logged-in user.</param>
        /// <returns>
        /// An <see cref="IdentityResult"/> indicating the success or failure of the deletion.
        /// </returns>
        public async Task<IdentityResult> DeleteUserAsync(string userId, string currentUserId)
        {
            return await accountRepository.DeleteUserAsync(userId, currentUserId);
        }

        /// <summary>
        /// Retrieves all users, paginated.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <returns>
        /// A collection of <see cref="ApplicationUser"/> objects.
        /// </returns>
        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync(int page, int pageSize)
        {
            return await accountRepository.GetAllUsersAsync(page, pageSize);
        }

        /// <summary>
        /// Retrieves the total count of users in the system.
        /// </summary>
        /// <returns>
        /// The total number of users.
        /// </returns>
        public async Task<int> GetTotalUserCount()
        {
            return await accountRepository.GetTotalUserCount();
        }

        /// <summary>
        /// Updates an existing user based on the provided edit data.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="editUserDto">The new user data.</param>
        /// <returns>
        /// An <see cref="IdentityResult"/> indicating the success or failure of the update.
        /// </returns>
        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user, EditUserDto editUserDto)
        {
            return await accountRepository.UpdateUserAsync(user, editUserDto);
        }

        /// <summary>
        /// Retrieves the available roles in the system.
        /// </summary>
        /// <returns>
        /// A collection of role names.
        /// </returns>
        public async Task<IEnumerable<string>> GetAvailableRolesAsync()
        {
            return await accountRepository.GetAvailableRolesAsync();
        }

        /// <summary>
        /// Finds a user by their email address asynchronously.
        /// </summary>
        /// <param name="email">The email of the user to find.</param>
        /// <returns>
        /// The <see cref="ApplicationUser"/> object if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<ApplicationUser> FindUserByEmailAsync(string email)
        {
            return await accountRepository.FindUserByEmailAsync(email);
        }

        /// <summary>
        /// Retrieves users in a paginated format, along with their roles.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <returns>
        /// A <see cref="PagedUsersViewModel"/> containing user data and pagination info.
        /// </returns>
        public async Task<PagedUsersViewModel> GetPagedUsersAsync(int page, int pageSize)
        {
            var users = await accountRepository.GetAllUsersAsync(page, pageSize);
            var totalUsers = await accountRepository.GetTotalUserCount();

            var userViewModels = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user); // Await the roles retrieval
                userViewModels.Add(new UserViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList() // Convert to list
                });
            }

            return new PagedUsersViewModel
            {
                Users = userViewModels,
                CurrentPage = page,
                PageSize = pageSize,
                TotalUsers = totalUsers
            };
        }

        /// <summary>
        /// Retrieves the editable view model for a user.
        /// </summary>
        /// <param name="userId">The ID of the user to edit.</param>
        /// <returns>
        /// An <see cref="EditUserDto"/> object with current user details.
        /// </returns>
        public async Task<EditUserDto> GetEditUserViewModelAsync(string userId)
        {
            var user = await accountRepository.GetUserByIdAsync(userId);
            var userRoles = await userManager.GetRolesAsync(user);

            return new EditUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = userRoles.FirstOrDefault(), // Get the current role
            };
        }
    }
}
