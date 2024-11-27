using Microsoft.AspNetCore.Mvc;
using UserAuthMicroservice.Model;
using UserAuthMicroservice.Repositories;

namespace UserAuthMicroservice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserRepository _repository;

        public UserController(UserRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var existingUser = await _repository.FindByUsernameAsync(user.Username);
            if (existingUser != null)
            {
                return BadRequest("Username is already taken.");
            }

            user.CreatedAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            await _repository.AddUserAsync(user);
            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User loginUser)
        {
            var existingUser = await _repository.FindByUsernameAsync(loginUser.Username);
            if (existingUser == null || existingUser.PasswordHash != loginUser.PasswordHash)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { message = "Login successful." });
        }

        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] User updatedUser)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.Username = updatedUser.Username;
            user.Email = updatedUser.Email;
            user.UpdatedAt = DateTime.Now;

            await _repository.UpdateUserAsync(user);
            return Ok(new { message = "Profile updated successfully." });
        }

        [HttpPut("change-password/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordModel model)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            if (user.PasswordHash != model.OldPasswordHash)
            {
                return BadRequest("Old password is incorrect.");
            }

            user.PasswordHash = model.NewPasswordHash;
            user.UpdatedAt = DateTime.Now;

            await _repository.UpdateUserAsync(user);
            return Ok(new { message = "Password changed successfully." });
        }

        [HttpGet("validate/{id}")]
        public async Task<IActionResult> ValidateUser(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(new { userId = user.Id, username = user.Username });
        }
    }
}
