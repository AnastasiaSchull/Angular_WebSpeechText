using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Angular_WebSpeechText.Models;

//Класс ControllerBase предоставляет базовую функциональность для API контроллеров и не включает поддержку вьюшек, что делает его более подходящим для API-центричных приложений
namespace Angular_WebSpeechText.Server.Controllers
{

    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserContext _context;

        public AccountController(UserContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterModel reg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userExists = _context.Users.Any(u => u.Login == reg.Login);
            if (userExists)
            {
                return BadRequest("User with this login already exists.");
            }

            // Создание нового пользователя
            User user = new User();

            user.FirstName = reg.FirstName;
            user.LastName = reg.LastName;
            user.Login = reg.Login;

            // Генерация соли
            byte[] saltbuf = new byte[16];
            RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(saltbuf);
           
            string salt = Convert.ToHexString(saltbuf);

            //переводим пароль в байт-массив  
            byte[] password = Encoding.Unicode.GetBytes(salt + reg.Password);
            //byte[] password = Encoding.UTF8.GetBytes(salt + reg.Password);
            //вычисляем хеш-представление в байтах  
            byte[] byteHash = SHA256.HashData(password);
          
            string passwordHash = Convert.ToHexString(byteHash);

            //user.Password = hash.ToString();
            user.Password = passwordHash;
            user.Salt = salt;

            _context.Users.Add(user);
            _context.SaveChanges();

            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }


        [HttpPost("login")]
        public ActionResult<User> Login(LoginModel logon)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Поиск пользователя по логину
                // var user = _context.Users.SingleOrDefault(u => u.Login == logon.Login);
                var user = _context.Users.FirstOrDefault(u => u.Login == logon.Login);
                if (user == null)
                {
                    // Пользователь с таким логином не найден
                    return NotFound("User not found.");
                }
                Console.WriteLine($"Stored password: {user.Password}");
                Console.WriteLine($"Stored salt: {user.Salt}");              

                // Хэшируем пароль для проверки
                byte[] passwordBytes = Encoding.Unicode.GetBytes(user.Salt + logon.Password);
                byte[] byteHash = SHA256.HashData(passwordBytes);
                string computedHash = Convert.ToHexString(byteHash);

                Console.WriteLine($"Computed hash: {computedHash}");

                if (user.Password != computedHash)
                {
                    return BadRequest("Wrong login or password!");
                }

                // Аутентификация прошла успешно
                return Ok(new { user.Id, user.FirstName, user.LastName,
                    Token = GenerateFakeToken(user)
                });
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Error in Login: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        private string GenerateFakeToken(User user)
        {
            // Пример генерации токена (можно заменить на JWT)
            return Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user.Id}:{user.Login}:{DateTime.UtcNow}"));
        }
       
    }
}

