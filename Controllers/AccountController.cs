using LiberaryManagmentSystem.Dtos;
using LiberaryManagmentSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LiberaryManagmentSystem.ViewModels;

public class AccountController(IUserService userService) : Controller
{
    /// <summary>
    /// Handles user registration.
    /// </summary>
    /// <param name="registerDto">User registration data.</param>
    /// <returns>Redirects to Login on success, or returns the view with validation errors.</returns>
    /// <remarks>POST: /Account/Register</remarks>
    [HttpPost]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return View(registerDto);
        }

        var result = await userService.RegisterUserWithRole(registerDto);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(registerDto);
        }

        return RedirectToAction("Login", "Account");
    }

    /// <summary>
    /// Displays the registration form.
    /// </summary>
    /// <returns>View for user registration.</returns>
    /// <remarks>GET: /Account/Register</remarks>
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    /// <summary>
    /// Handles user login.
    /// </summary>
    /// <param name="loginDto">User login data.</param>
    /// <returns>Redirects to home on success, or returns the view with validation errors.</returns>
    /// <remarks>POST: /Account/Login</remarks>
    [HttpPost]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return View(loginDto);
        }

        var signInResult = await userService.LoginUser(loginDto);

        if (!signInResult.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
            return View(loginDto);
        }

        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Displays the login form.
    /// </summary>
    /// <returns>View for user login.</returns>
    /// <remarks>GET: /Account/Login</remarks>
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Home");
        }
        return View();
    }

    /// <summary>
    /// Logs out the current user.
    /// </summary>
    /// <returns>Redirects to the login page.</returns>
    /// <remarks>POST: /Account/Logout</remarks>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await userService.LogoutUser();
        return RedirectToAction("Login", "Account");
    }

    /// <summary>
    /// Updates the user's profile.
    /// </summary>
    /// <param name="updateUserDto">User profile data to be updated.</param>
    /// <returns>Redirects to ProfileDetails on success, or returns the view with validation errors.</returns>
    /// <remarks>POST: /Account/UpdateProfile</remarks>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UpdateProfile(UpdateUserDto updateUserDto)
    {
        if (!ModelState.IsValid)
        {
            return View(updateUserDto);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        
        var result = await userService.UpdateUserProfile(userId, updateUserDto);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(updateUserDto);
        }

        TempData["SuccessMessage"] = "Profile updated successfully!";
        return RedirectToAction("ProfileDetails");
    }

    /// <summary>
    /// Displays the profile update form.
    /// </summary>
    /// <returns>View for updating user profile.</returns>
    /// <remarks>GET: /Account/UpdateProfile</remarks>
    [HttpGet]
    [Authorize]
    public IActionResult UpdateProfile()
    {
        return View();
    }

    /// <summary>
    /// Changes the user's password.
    /// </summary>
    /// <param name="changePasswordDto">User password data.</param>
    /// <returns>Redirects to home on success, or returns the view with validation errors.</returns>
    /// <remarks>POST: /Account/ChangePassword</remarks>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        if (!ModelState.IsValid)
        {
            return View(changePasswordDto);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var result = await userService.ChangeUserPasswordAsync(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
            return View(changePasswordDto);
        }

        TempData["SuccessMessage"] = "Password changed successfully!";
        await userService.LogoutUser();
        return RedirectToAction("Index", "Home");
    }

    /// <summary>
    /// Displays the change password form.
    /// </summary>
    /// <returns>View for changing user password.</returns>
    /// <remarks>GET: /Account/ChangePassword</remarks>
    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    /// <summary>
    /// Displays the user's profile details.
    /// </summary>
    /// <returns>View with user's profile data.</returns>
    /// <remarks>GET: /Account/ProfileDetails</remarks>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ProfileDetails()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        var user = await userService.GetUserByIdAsync(userId);

        var profileViewModel = new ProfileDetailsViewModel()
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            MembershipDate = user.MembershipDate
        };

        return View(profileViewModel);
    }

    /// <summary>
    /// Deletes a user account.
    /// </summary>
    /// <param name="userId">The ID of the user to delete.</param>
    /// <returns>Success message.</returns>
    /// <remarks>DELETE: /Account/DeleteUser</remarks>
    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
        await userService.DeleteUserAsync(userId, currentUserId);

        return Ok(new { Message = "User deleted successfully!" });
    }

    /// <summary>
    /// Retrieves all users in a paginated format.
    /// </summary>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="pageSize">Number of users per page.</param>
    /// <returns>View with paginated users.</returns>
    /// <remarks>GET: /Account/GetAllUsers</remarks>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers(int page = 1, int pageSize = 10)
    {
        if (page < 1 || pageSize < 1)
        {
            TempData["ErrorMessage"] = "Invalid pagination parameters.";
            return RedirectToAction("ErrorPage");
        }

        var pagedUsersViewModel = await userService.GetPagedUsersAsync(page, pageSize);

        if (pagedUsersViewModel.Users.Count == 0)
        {
            TempData["ErrorMessage"] = "No users found.";
            return RedirectToAction("ErrorPage");
        }

        return View(pagedUsersViewModel);
    }

    /// <summary>
    /// Displays the edit user form.
    /// </summary>
    /// <param name="userId">The ID of the user to edit.</param>
    /// <returns>View for editing user details.</returns>
    /// <remarks>GET: /Account/EditUser</remarks>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditUser(string userId)
    {
        var editUserViewModel = await userService.GetEditUserViewModelAsync(userId);
        return View(editUserViewModel);
    }

    /// <summary>
    /// Handles the update of user details.
    /// </summary>
    /// <param name="editUserDto">Updated user details.</param>
    /// <returns>Redirects to user list on success, or returns the view with validation errors.</returns>
    /// <remarks>POST: /Account/EditUser</remarks>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> EditUser(EditUserDto editUserDto)
    {
        if (!ModelState.IsValid)
        {
            return View(editUserDto);
        }

        var user = await userService.FindUserByEmailAsync(editUserDto.Email ?? "");
        if (user == null)
        {
            ModelState.AddModelError("", "User not found.");
            return View(editUserDto);
        }

        var resultUpdate = await userService.UpdateUserAsync(user, editUserDto);

        if (!resultUpdate.Succeeded)
        {
            foreach (var error in resultUpdate.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(editUserDto);
        }

        TempData["SuccessMessage"] = "User updated successfully!";
        return RedirectToAction("GetAllUsers");
    }

    /// <summary>
    /// Displays the Access Denied page.
    /// </summary>
    /// <returns>View for access denied message.</returns>
    /// <remarks>GET: /Account/AccessDenied</remarks>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
