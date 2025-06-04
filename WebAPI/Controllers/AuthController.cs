using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Context;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;              // Configuración de la app (leer claves JWT, etc.)
            _context = context;            // Contexto de la base de datos (Entity Framework)
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Buscar usuario con las credenciales proporcionadas
            var user = _context.Users.FirstOrDefault(u =>
                u.Username == request.UserName && u.Password == request.Password);

            if (user == null)
            {
                // Si no existe, se responde con Unauthorized
                var errorResponse = new Response<string>("Credenciales inválidas");
                return Unauthorized(errorResponse);
            }

            // Si las credenciales son correctas, se genera el token JWT

            var tokenHandler = new JwtSecurityTokenHandler(); // Manejador para crear tokens
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]); // Se obtiene la clave secreta desde la configuración

            // Se definen los claims (información incluida en el token)
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),                 // Nombre del usuario
                new Claim(ClaimTypes.NameIdentifier, user.PKUser.ToString()), // ID del usuario
                new Claim(ClaimTypes.Role, user.FKRol?.ToString() ?? "")  // Rol del usuario (puede ser nulo)
            };

            // Descriptor del token: qué incluye, cuándo expira, cómo se firma
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Se agregan los claims al token
                Expires = DateTime.UtcNow.AddHours(1), // El token expira en 1 hora
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),             // Clave de firma
                    SecurityAlgorithms.HmacSha256Signature)    // Algoritmo de firma
            };

            // Se crea y escribe el token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Se responde con el token y mensaje de éxito
            var successResponse = new Response<string>(tokenString, "Login exitoso");
            return Ok(successResponse);
        }
    }
}
