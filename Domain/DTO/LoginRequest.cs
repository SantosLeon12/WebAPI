namespace Domain.DTO
{
    // Esta clase representa el modelo que se recibe en el cuerpo del login (JSON)
    public class LoginRequest
    {
        public string UserName { get; set; }   // Nombre de usuario
        public string Password { get; set; }   // Contraseña
    }
}
