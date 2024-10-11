using Microsoft.AspNetCore.Identity;
using LiberaryManagmentSystem.Dtos;
using LiberaryManagmentSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LiberaryManagmentSystem.Repository
{
    /// <summary>
    /// Repository for managing user accounts, including registration, login, and user profile updates.
    /// </summary>
    public class AccountRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext context) : Repository<ApplicationUser>(context)
    {
        
        //! CREATE: Register a new user
        /// <summary>
        /// Registers a new user and returns the result of the registration process.
        /// </summary>
        /// <param name="registerDto">The registration data transfer object.</param>
        /// <returns>The result of the registration attempt.</returns>
        public async Task<IdentityResult> RegisterUser(RegisterDto registerDto)
        {
            var newUser = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName.Trim(),
                LastName = registerDto.LastName.Trim(),
                MembershipDate = registerDto.MembershipDate,
            };
            var result = await userManager.CreateAsync(newUser, registerDto.Password);
            return result;
        }

        /// <summary>
        /// Updates the user profile with the provided information.
        /// </summary>
        /// <param name="user">The user to update.</param>
        /// <param name="editUserDto">The DTO containing the updated user information.</param>
        /// <returns>The result of the update attempt.</returns>
        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user, EditUserDto? editUserDto)
        {
            user.FirstName = editUserDto!.FirstName!;
            user.LastName = editUserDto!.LastName!;
            user.Email = editUserDto.Email;

            if (!string.IsNullOrWhiteSpace(editUserDto.Password))
            {
                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, editUserDto.Password);
            }

            var result = await userManager.UpdateAsync(user);

            if (editUserDto.Role != null)
            {
                var currentRoles = await userManager.GetRolesAsync(user);
                var removeResult = await userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeResult.Succeeded)
                {
                    return removeResult; // Return if removing roles fails
                }

                var addResult = await userManager.AddToRoleAsync(user, editUserDto.Role);
                if (!addResult.Succeeded)
                {
                    return addResult; // Return if adding roles fails
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user associated with the email, or null if not found.</returns>
        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Retrieves a list of available roles.
        /// </summary>
        /// <returns>A list of role names.</returns>
        public async Task<List<string>> GetAvailableRolesAsync()
        {
            return await context.Roles.Select(r => r.Name).ToListAsync();
        }

        //! READ: Get user by email
        /// <summary>
        /// Finds a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<ApplicationUser?> FindUserByEmailAsync(string email)
        {
            return await userManager.FindByEmailAsync(email);
        }

        //! READ: Get all users (paginated)
        /// <summary>
        /// Retrieves a paginated list of all users.
        /// </summary>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="pageSize">The number of users per page.</param>
        /// <returns>A list of users on the specified page.</returns>
        public async Task<List<ApplicationUser>> GetAllUsersAsync(int page, int pageSize)
        {
            return await userManager.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        //! READ: Get Total Number of Users
        /// <summary>
        /// Gets the total count of users in the system.
        /// </summary>
        /// <returns>The total number of users.</returns>
        public async Task<int> GetTotalUserCount()
        {
            return await userManager.Users.CountAsync();
        }

        //! READ: Get user by ID
        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await userManager.FindByIdAsync(userId);
        }

        //! UPDATE: Update user profile (excluding password)
        /// <summary>
        /// Updates a user's profile information, excluding the password.
        /// </summary>
        /// <param name="userId">The ID of the user to update.</param>
        /// <param name="updateUserDto">The DTO containing updated user profile information.</param>
        /// <returns>The result of the update attempt.</returns>
        public async Task<IdentityResult> UpdateUserProfile(string userId, UpdateUserDto updateUserDto)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (!string.IsNullOrEmpty(updateUserDto.FirstName))
            {
                user.FirstName = updateUserDto.FirstName.Trim();
            }

            if (!string.IsNullOrEmpty(updateUserDto.LastName))
            {
                user.LastName = updateUserDto.LastName.Trim();
            }

            if (!string.IsNullOrEmpty(updateUserDto.Email))
            {
                user.Email = updateUserDto.Email;
                user.UserName = updateUserDto.Email;
            }

            return await userManager.UpdateAsync(user);
        }

        //! UPDATE: Change user password
        /// <summary>
        /// Changes the password of the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose password is to be changed.</param>
        /// <param name="currentPassword">The current password of the user.</param>
        /// <param name="newPassword">The new password to set.</param>
        /// <returns>The result of the password change attempt.</returns>
        public async Task<IdentityResult> ChangeUserPasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await userManager.FindByIdAsync(userId);
            return await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        //! DELETE: delete the user (admin only!)
        /// <summary>
        /// Deletes a user from the system.
        /// </summary>
        /// <param name="userId">The ID of the user to delete.</param>
        /// <param name="currentUserId">The ID of the user performing the deletion.</param>
        /// <returns>The result of the delete attempt.</returns>
        public async Task<IdentityResult> DeleteUserAsync(string userId, string currentUserId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId), "User ID cannot be null or empty.");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User not found.");
            }

            if (userId == currentUserId)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "You cannot delete your own account."
                });
            }

            var result = await userManager.DeleteAsync(user);
            return result;
        }

        //! LOGIN: Log in the user
        /// <summary>
        /// Logs in a user using their credentials.
        /// </summary>
        /// <param name="loginDto">The login data transfer object containing user credentials.</param>
        /// <returns>The result of the login attempt.</returns>
        public async Task<SignInResult> LoginUser(LoginDto loginDto)
        {
            var result = await signInManager.PasswordSignInAsync(loginDto.Email, loginDto.Password, isPersistent: false, lockoutOnFailure: false);
            return result;
        }

        //! LOGOUT
        /// <summary>
        /// Logs out the currently signed-in user.
        /// </summary>
        public async Task LogoutUser()
        {
            await signInManager.SignOutAsync();
        }
    }
}
