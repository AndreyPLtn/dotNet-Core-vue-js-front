using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using AccountingApp.Data;

namespace AccountingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ApplicationDbContext _context;

        public UserController(ILogger<UserController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Register(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) //проверка заполнени€ им€/пароль
            {
                return BadRequest("Error 400. «аполните пустые пол€");
            }

            if (_context.Users.Any(u => u.Username == username)) //проверка свободного имени
            {
                return Conflict("Error 409. ѕользователь с указанным именем уже существует");
            };

            var user = new Models.User() //создаем нового пользовател€
            {
                Username = username
            };
                      
            using (var sha256Hash = SHA256.Create()) //значение в пространстве
            {
                byte[] passwordBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                user.PasswordHash = BitConverter.ToString(passwordBytes).Replace("-", String.Empty);
            };

            _context.Users.Add(user); //добавл€ем user
            _context.SaveChanges(); //сохран€ем в бд

            _logger.LogInformation($"”спешна€ регистраци€ пользовател€ {username}");
            return Ok($"ѕользователь {username} успешно зарегистрирован, ’еш: {user.PasswordHash}"); //возврат успеха с ’ешом
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var user = _context.Users.First(u => u.Username == username); //поиск по имени в бд
            if (user == null) //проверка существовани€ пользовател€
            {
                //возврат ошибку, если не нашли пользовател€
                return NotFound("Error 404: ѕользовател€ с таким именем не существует");
            }

            //создаем экземпл€р хеша парол€ на проверку парол€
            using (var sha256Hash = SHA256.Create())
            {
                byte[] passwordBytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                string hashedPassword = BitConverter.ToString(passwordBytes).Replace("-", string.Empty);

                if (hashedPassword != user.PasswordHash)
                {
                    return Unauthorized("Error 403: Ќеверный пароль");
                }
            }

            var claims = new List<Claim> { new Claim(ClaimTypes.Name, username) };
            // создаем JWT-токен
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TytNeHitrimSposobomYaKlady256Bit"));
            var jwt = new JwtSecurityToken(
                    issuer: "tgk",
                    audience: "TgkWebApp",
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(2),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            //”спешный успех в случае успеха
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Ok(new { token });
        }
    }
}