using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.IServices;
using WebAPI.Services.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RolController : Controller
    {
        private readonly IRolServices _rolServices;
        //Instancia la clase IRolServices en todo el proyecto para usar todos lo metodos creados
        public RolController(IRolServices rolServices)
        {
            _rolServices = rolServices;
        }

        // Obtener todos los roles
        [HttpGet]
        public async Task<IActionResult> GetRols()
        {
            // Llama al servicio para obtener la lista de roles
            var rols = await _rolServices.GetRols();

            // Retorna la lista de roles con estado 200 OK
            return Ok(rols);
        }

        // Obtener un rol por ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRol(int id)
        {
            // Llama al servicio para obtener un rol específico por ID
            var rol = await _rolServices.GetByIdRol(id);

            // Si no se encuentra el rol, retorna 404 Not Found
            if (rol == null)
                return NotFound($"No se encontró un rol con ID {id}.");

            // Retorna el rol encontrado con estado 200 OK
            return Ok(rol);
        }

        // Crear un nuevo rol
        [HttpPost("crear")]
        public async Task<IActionResult> PostRol([FromBody] Rol request)
        {
            // Verifica que el modelo recibido sea válido
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Llama al servicio para crear el nuevo rol
            var createdRol = await _rolServices.CreateRol(request);

            // Retorna estado 201 Created con la ruta al nuevo recurso
            return CreatedAtAction(nameof(GetRol), new { id = createdRol.PKRol }, createdRol);
        }

        // Actualizar un rol existente
        [HttpPut("editar/{id:int}")]
        public async Task<IActionResult> PutRol(int id, [FromBody] Rol request)
        {
            // Verifica que el ID de la URL coincida con el del cuerpo del request
            if (id != request.PKRol)
                return BadRequest("El ID en la URL no coincide con el ID del cuerpo.");

            // Llama al servicio para actualizar el rol
            var updatedRol = await _rolServices.EditRol(request);

            // Si no se pudo actualizar (rol no encontrado), retorna 404 Not Found
            if (updatedRol == null)
                return NotFound($"No se pudo actualizar el rol con ID {id}.");

            // Retorna el rol actualizado con estado 200 OK
            return Ok(updatedRol);
        }

        // Eliminar un rol
        [HttpDelete("eliminar/{id:int}")]
        public async Task<IActionResult> DeleteRol(int id)
        {
            // Llama al servicio para eliminar el rol por ID
            var deleted = await _rolServices.DeleteRol(id);

            // Si no se eliminó (rol no encontrado), retorna 404 Not Found
            if (!deleted)
                return NotFound($"No se encontró el rol con ID {id} para eliminar.");

            // Retorna mensaje de éxito con estado 200 OK
            return Ok(new { message = $"Rol con ID {id} eliminado correctamente." });
        }

    }
}
