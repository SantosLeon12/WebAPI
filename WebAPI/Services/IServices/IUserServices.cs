using Domain.DTO;
using Domain.Entities;

namespace WebAPI.Services.IServices
{
    // Interfaz que define los servicios relacionados con la entidad Rol.
    public interface IUserServices
    {
        Task<List<User>> GetUsers(); // Obtiene la lista de todos los usuarios.
        Task<User> GetByIdUser(int id); // Obtiene un usuario por su identificador.
        Task<User> CreateUser(UserRequest i); // Crea un nuevo usuario en la base de datos.
        Task<User> EditUser(UserRequest i); // Edita los datos de un usuario existente.
        Task<bool> DeleteUser(int id); // Elimina un usuario por su identificador.
    }
}
