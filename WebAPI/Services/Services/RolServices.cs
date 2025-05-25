using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using WebAPI.Context;
using WebAPI.Services.IServices;

namespace WebAPI.Services.Services
{
    public class RolServices : IRolServices
    {
        private readonly ApplicationDbContext _context;
        public RolServices(ApplicationDbContext context)
        {
            _context = context;
        }
        // Obtiene todos los roles existentes en la base de datos
        public async Task<List<Rol>> GetRols()
        {
            try
            {
                // Retorna una lista de todos los registros de la tabla Roles
                return await _context.Roles.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Surgió un error: " + ex.Message);
            }
        }

        // Obtiene un rol específico por su ID
        public async Task<Rol> GetByIdRol(int id)
        {
            try
            {
                // Busca el rol con el ID proporcionado
                Rol response = await _context.Roles
                    .FirstOrDefaultAsync(x => x.PKRol == id);
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Surgió un error: " + ex.Message);
            }
        }

        // Crea un nuevo rol en la base de datos
        public async Task<Rol> CreateRol(Rol i)
        {
            try
            {
                // Crea un nuevo objeto de tipo Rol con los datos proporcionados
                Rol request = new Rol()
                {
                    Name = i.Name
                };

                // Agrega el nuevo rol a la base de datos de forma asincrónica
                var result = await _context.Roles.AddAsync(request);

                // Guarda los cambios en la base de datos
                _context.SaveChanges();

                return request;
            }
            catch (Exception ex)
            {
                throw new Exception("Surgió un error: " + ex.Message);
            }
        }

        // Edita un rol existente
        public async Task<Rol> EditRol(Rol i)
        {
            try
            {
                // Busca el rol existente en la base de datos por su ID
                Rol request = await _context.Roles.FindAsync(i.PKRol);

                // Actualiza el nombre del rol con el nuevo valor
                request.Name = i.Name;

                // Marca la entidad como modificada
                _context.Entry(request).State = EntityState.Modified;

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                return request;
            }
            catch (Exception ex)
            {
                throw new Exception("Surgió un error: " + ex.Message);
            }
        }

        // Elimina un rol por su ID.
        public async Task<bool> DeleteRol(int id)
        {
            try
            {
                // Busca al rol en la base de datos por su ID
                var rol = await _context.Roles.FindAsync(id);
                // Si no se encuentra el rol, retorna false
                if (rol == null)
                    return false;

                // Elimina el rol encontrado del contexto
                _context.Roles.Remove(rol);
                // Guarda los cambios realizados en la base de datos de forma asincrónica
                await _context.SaveChangesAsync();
                // Retorna true indicando que la eliminación fue exitosa
                return true;
            }
            catch (Exception ex)
            {
                // Lanza una nueva excepción si ocurre un error durante el proceso
                throw new Exception("Surgió un error: " + ex.Message);
            }
        }
    }
}
