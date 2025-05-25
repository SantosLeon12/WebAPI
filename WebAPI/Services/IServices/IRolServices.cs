using Domain.Entities;

namespace WebAPI.Services.IServices
{
    // Interfaz que define los servicios relacionados con la entidad Rol.
    public interface IRolServices
    {

        Task<List<Rol>> GetRols(); // Obtiene la lista de todos los roles.
        Task<Rol> GetByIdRol(int id); // Obtiene un rol por su identificador.
        Task<Rol> CreateRol(Rol i); // Crea un nuevo rol en la base de datos.
        Task<Rol> EditRol(Rol i); // Edita los datos de un rol existente.
        Task<bool> DeleteRol(int id); // Elimina un rol por su identificador.
    }
}
