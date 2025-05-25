using Domain.DTO; // Importa los Data Transfer Objects (DTO) definidos en el dominio
using Domain.Entities; // Importa las entidades del dominio, como User
using Microsoft.EntityFrameworkCore; // Para trabajar con EF Core y sus funcionalidades
using WebAPI.Context; // Contexto de base de datos configurado para la aplicación
using WebAPI.Services.IServices; // Interfaz del servicio de usuarios

namespace WebAPI.Services.Services
{
    // Implementación del servicio de usuarios
    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _context;

        // Inyección del contexto de base de datos
        public UserServices(ApplicationDbContext context)
        {
            _context = context;
        }

        /// Obtiene la lista de todos los usuarios, incluyendo sus roles relacionados.
        public async Task<List<User>> GetUsers()
        {
            try
            {
                return await _context.Users
                    .Include(u => u.Roles) // Incluye los datos del rol en la consulta
                    .ToListAsync(); // Convierte el resultado en una lista asincrónicamente
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los usuarios. Detalles: " + ex.Message);
            }
        }

        /// Obtiene un usuario por su ID, incluyendo su rol si se solicita.
        public async Task<User> GetByIdUser(int id)
        {
            try
            {
                // Usa método auxiliar reutilizable con opción de incluir roles
                return await FindUserByIdAsync(id, includeRoles: true);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener el usuario. Detalles: " + ex.Message);
            }
        }

        /// Crea un nuevo usuario a partir de los datos recibidos del DTO.
        public async Task<User> CreateUser(UserRequest i)
        {
            try
            {
                // Validación básica de campos requeridos
                if (string.IsNullOrWhiteSpace(i.Name) ||
                    string.IsNullOrWhiteSpace(i.Username) ||
                    string.IsNullOrWhiteSpace(i.Password))
                {
                    throw new ArgumentException("Todos los campos son requeridos.");
                }

                var user = new User
                {
                    Name = i.Name.Trim(),
                    Username = i.Username.Trim(),
                    Password = i.Password, // En producción, se debe hashear
                    FKRol = i.FKRol
                };

                // Agrega y guarda el nuevo usuario en la base de datos
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el usuario. Detalles: " + ex.Message);
            }
        }
        /// Edita un usuario existente. Solo actualiza los campos que vienen en el DTO.
        public async Task<User> EditUser(UserRequest i)
        {
            try
            {
                var user = await FindUserByIdAsync(i.PKUser);
                if (user == null)
                    throw new Exception("Usuario no encontrado.");

                // Solo actualiza campos si se proporcionan (evita sobrescribir con nulos o vacíos)
                if (!string.IsNullOrWhiteSpace(i.Name))
                    user.Name = i.Name.Trim();

                if (!string.IsNullOrWhiteSpace(i.Username))
                    user.Username = i.Username.Trim();

                if (!string.IsNullOrWhiteSpace(i.Password))
                    user.Password = i.Password;

                // Si se proporciona un FKRol válido, se actualiza
                if (i.FKRol > 0)
                    user.FKRol = i.FKRol;

                // Marca la entidad como modificada y guarda cambios
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar el usuario. Detalles: " + ex.Message);
            }
        }

        /// Elimina un usuario por su ID.
        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var user = await FindUserByIdAsync(id);
                if (user == null)
                    return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar el usuario. Detalles: " + ex.Message);
            }
        }

        /// Método auxiliar reutilizable para encontrar un usuario por su ID,
        /// con opción de incluir los roles asociados.
        private async Task<User?> FindUserByIdAsync(int id, bool includeRoles = false)
        {
            return includeRoles
                ? await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.PKUser == id)
                : await _context.Users.FindAsync(id);
        }
    }
}
