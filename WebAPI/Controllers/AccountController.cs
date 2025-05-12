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
    /// <summary>
    /// Контроллер для управления аутентификацией и профилями пользователей
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;

        /// <summary>
        /// Инициализирует новый экземпляр <see cref="AccountController"/>
        /// </summary>
        /// <param name="userManager">Менеджер пользователей</param>
        /// <param name="signInManager">Менеджер аутентификации</param>
        /// <param name="configuration">Конфигурация приложения</param>
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            IConfiguration configuration, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Регистрирует нового пользователя
        /// </summary>
        /// <param name="model">Данные для регистрации</param>
        /// <returns>Результат операции регистрации</returns>
        /// <response code="200">Пользователь успешно зарегистрирован</response>
        /// <response code="400">Некорректные данные запроса</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            _logger.LogInformation($"Новый пользователь регистрируется в системе. Логин: {model.UserName} ");
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

            var user = new User { UserName = model.UserName, FullName = model.FullName, PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Новый пользователь зарегистрирован. Логин: {model.UserName} ");
                await _userManager.AddToRoleAsync(user, "User");
                return Ok(new { Message = "User registered successfully" });
            }
            return BadRequest(result.Errors);
        }

        /// <summary>
        /// Выполняет вход пользователя в систему
        /// </summary>
        /// <param name="model">Данные для входа</param>
        /// <returns>JWT-токен и информация о пользователе</returns>
        /// <response code="200">Успешный вход</response>
        /// <response code="400">Некорректные данные запроса</response>
        /// <response code="401">Неверные учетные данные</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            _logger.LogInformation($"Пользователь с логином {model.UserName} пытается войти в систему");
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                var token = GenerateJwtToken(user);
                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation($"{model.UserName} вошел в систему.");
                return Ok(new
                {
                    token = token,
                    userName = user.UserName,
                    userRole = roles.FirstOrDefault()
                });
            }

            return Unauthorized();
        }

        /// <summary>
        /// Выполняет выход пользователя из системы
        /// </summary>
        /// <returns>Результат операции выхода</returns>
        /// <response code="200">Успешный выход</response>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} выходит из системы");
            await _signInManager.SignOutAsync();
            return Ok(new { Message = "User logged out successfully" });
        }

        /// <summary>
        /// Проверяет валидность токена текущего пользователя
        /// </summary>
        /// <returns>Информация о текущем пользователе</returns>
        /// <response code="200">Токен валиден</response>
        /// <response code="401">Недействительный токен</response>
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateToken()
        {
            _logger.LogInformation($"Вызывается метод валидации токена");
            User usr = await _userManager.GetUserAsync(HttpContext.User);
            if (usr == null)
            {
                return Unauthorized(new { message = "Вы Гость. Пожалуйста, выполните вход" });
            }
            IList<string> roles = await _userManager.GetRolesAsync(usr);
            string userRole = roles.FirstOrDefault();
            return Ok(new { message = "Сессия активна", userName = usr.UserName, userRole });
        }

        /// <summary>
        /// Получает профиль текущего пользователя с бронированиями
        /// </summary>
        /// <returns>Полная информация о профиле пользователя</returns>
        /// <response code="200">Профиль успешно получен</response>
        /// <response code="401">Пользователь не авторизован</response>
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} получает информацию о текущем пользователе в системе");
            var user = await _userManager.Users
            .Include(u => u.Reservation)
                .ThenInclude(r => r.Object)
            .Include(u => u.Reservation)
                .ThenInclude(r => r.ResStatus)
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(HttpContext.User));

            if (user != null)
            {
                user.Reservation = user.Reservation.OrderByDescending(r => r.Id).ToList();
            }

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

        /// <summary>
        /// Обновляет профиль текущего пользователя
        /// </summary>
        /// <param name="model">Новые данные профиля</param>
        /// <returns>Результат обновления профиля</returns>
        /// <response code="200">Профиль успешно обновлен</response>
        /// <response code="400">Некорректные данные запроса</response>
        /// <response code="401">Пользователь не авторизован</response>
        [Authorize]
        [HttpPut("editprofile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileModel model)
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} редактирует свой профиль");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized(new { message = "Пользователь не найден" });
            }

            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"{currUser} успешно внес изменения в свой профиль");
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

        /// <summary>
        /// Получает список всех пользователей системы
        /// </summary>
        /// <returns>Список пользователей с их ролями</returns>
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var currUser = User.Identity.IsAuthenticated ? User.Identity.Name : "Неавторизованный пользователь";
            _logger.LogInformation($"{currUser} получает информацию о всех пользователях системы");
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

        /// <summary>
        /// Генерирует JWT-токен для пользователя
        /// </summary>
        /// <param name="user">Пользователь</param>
        /// <returns>Сгенерированный JWT-токен</returns>
        private string GenerateJwtToken(User user)
        {
            _logger.LogInformation($"Для пользователя {user.UserName} происходит генерация токена");
            var claims = new List<Claim>
            { //саб - субъект, jti - идентификатор токена
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName), //клейм - пара ключ-значение хранящая инфу о пользователе
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var roles = _userManager.GetRolesAsync(user).Result;
            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role)));
            // ключ для подписи. симметричный (один для подписи и проверки) 
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            //подпись токена(параметры,алгоритм)
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);//токен в виде строки
        }
    }
}