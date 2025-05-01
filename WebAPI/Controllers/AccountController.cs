using ASPNETCore.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Model;

namespace ProjectManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState невалидна:");
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"Поле: {kvp.Key}, Ошибка: {error.ErrorMessage}");
                    }
                }

                return BadRequest(ModelState);
            }
            //var user = new User { UserName = model.UserName, Email = model.Email };
            var user = new User { UserName = model.UserName, FullName = model.FullName, PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                //await _userManager.AddToRoleAsync(user, model.Role);
                await _userManager.AddToRoleAsync(user, "User");
                return Ok(new { Message = "User registered successfully" });
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var token = GenerateJwtToken(user);
                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    token = token,
                    userName = user.UserName,
                    userRole = roles.FirstOrDefault()
                });
            }

            return Unauthorized();
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "User logged out successfully" });
        }

        [HttpGet("validate")]
        public async Task<IActionResult> ValidateToken()
        {
            User usr = await _userManager.GetUserAsync(HttpContext.User);
            if (usr == null)
            {
                return Unauthorized(new { message = "Вы Гость. Пожалуйста, выполните вход" });
            }
            IList<string> roles = await _userManager.GetRolesAsync(usr);
            string userRole = roles.FirstOrDefault();
            return Ok(new { message = "Сессия активна", userName = usr.UserName, userRole });

        }
        //[Authorize]
        //[HttpGet("profile")]
        //public async Task<IActionResult> GetCurrentUser()
        //{
        //    var user = await _userManager.GetUserAsync(HttpContext.User);
        //    if (user == null)
        //    {
        //        return Unauthorized(new { message = "Пользователь не найден" });
        //    }

        //    var roles = await _userManager.GetRolesAsync(user);

        //    return Ok(new
        //    {
        //        id = user.Id,
        //        userName = user.UserName,
        //        fullName = user.FullName,
        //        phoneNumber = user.PhoneNumber,
        //        //userRole = roles.FirstOrDefault()
        //    });
        //}
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.Users
                .Include(u => u.Reservation)
                    .ThenInclude(r => r.Object)   // Загрузка связанных объектов
                .Include(u => u.Reservation)
                    .ThenInclude(r => r.ResStatus) // Загрузка статусов
                .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(HttpContext.User));

            if (user == null)
            {
                return Unauthorized(new { message = "Пользователь не найден" });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userProfile = new Dictionary<string, object>
            {
                { "id", user.Id },
                { "userName", user.UserName },
                { "fullName", user.FullName },
                { "phoneNumber", user.PhoneNumber },
                { "reservations", user.Reservation?.Select(r => new Dictionary<string, object>
                    {
                        { "id", r.Id },
                        { "startDate", r.StartDate },
                        { "endDate", r.EndDate },
                        { "userId", r.UserId },
                        { "objectId", r.ObjectId },
                        { "resStatusId", r.ResStatusId },
                        { "object", r.Object != null ? new Dictionary<string, object>
                            {
                                { "id", r.Object.Id },
                                { "street", r.Object.Street },
                                { "building", r.Object.Building },
                                { "roomnum", r.Object.Roomnum }
                            } : null },
                        { "resStatus", r.ResStatus != null ? new Dictionary<string, object>
                            {
                                { "id", r.ResStatus.Id },
                                { "statusType", r.ResStatus.StatusType }
                            } : null }
                    }) },
                { "userRole", roles.FirstOrDefault() }
             };

            return Ok(userProfile);
        }


        [Authorize]
        [HttpPut("editprofile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized(new { message = "Пользователь не найден" });
            }

            // Обновляем только разрешенные поля
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return Ok(new
                {
                    message = "Профиль успешно обновлен",
                    userName = user.UserName,
                    fullName = user.FullName,
                    phoneNumber = user.PhoneNumber
                });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = new List<object>();
            var allUsers = await _userManager.Users.AsNoTracking().ToListAsync();

            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                users.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.FullName,
                    user.PhoneNumber,
                    Roles = roles
                });
            }

            return Ok(users);
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
