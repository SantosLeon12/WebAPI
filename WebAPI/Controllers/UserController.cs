using Microsoft.AspNetCore.Mvc;
using WebAPI.Services.IServices;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.DTO;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        //Instancia la clase IUserServices en todo el proyecto para usar todos lo metodos creados
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        // Obtener todos los usuarios
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            // Llama al servicio para obtener todos los usuarios
            var users = await _userServices.GetUsers();

            // Retorna la lista de usuarios con estado 200 OK
            return Ok(users);
        }

        // Obtener un usuario por ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUser(int id)
        {
            // Llama al servicio para obtener un usuario por ID
            var user = await _userServices.GetByIdUser(id);

            // Si no se encuentra, retorna 404 Not Found
            if (user == null)
                return NotFound($"No se encontró un usuario con ID {id}.");

            // Retorna el usuario encontrado con estado 200 OK
            return Ok(user);
        }

        // Crear un nuevo usuario
        [HttpPost("crear")]
        public async Task<IActionResult> PostUser([FromBody] UserRequest request)
        {
            // Verifica si el modelo recibido es válido
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Llama al servicio para crear el usuario
            var createdUser = await _userServices.CreateUser(request);

            // Retorna estado 201 Created con la ruta del nuevo recurso
            return CreatedAtAction(nameof(GetUser), new { id = createdUser.PKUser }, createdUser);
        }

        // Actualizar un usuario existente
        [HttpPut("editar/{id:int}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] UserRequest request)
        {
            // Verifica que el ID en la URL coincida con el del cuerpo del request
            if (id != request.PKUser)
                return BadRequest("El ID en la URL no coincide con el ID del cuerpo.");

            // Llama al servicio para editar el usuario
            var updatedUser = await _userServices.EditUser(request);

            // Si no se pudo actualizar (usuario no encontrado), retorna 404 Not Found
            if (updatedUser == null)
                return NotFound($"No se pudo actualizar el usuario con ID {id}.");

            // Retorna el usuario actualizado con estado 200 OK
            return Ok(updatedUser);
        }

        // Eliminar un usuario
        [HttpDelete("eliminar/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Llama al servicio para eliminar el usuario por ID
            var deleted = await _userServices.DeleteUser(id);

            // Si no se eliminó (usuario no encontrado), retorna 404 Not Found
            if (!deleted)
                return NotFound($"No se encontró el usuario con ID {id} para eliminar.");

            // Retorna mensaje de éxito con estado 200 OK
            return Ok(new { message = $"Usuario con ID {id} eliminado correctamente." });
        }
    }
}
