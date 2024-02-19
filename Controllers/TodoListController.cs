using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using To_Do_List_Back.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Claims;

namespace To_Do_List_Back.Controllers
{
    public class TodoListDto
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TodoListController : ControllerBase
    {
        private readonly ListContext _context;

        public TodoListController(ListContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<List>>> GetTodoLists()
        {
            try
            {
                var userId = ExtractUserIdFromToken(HttpContext);

                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var user = await _context.User.FindAsync(userId);
                if (user == null)
                {
                    return Unauthorized("User not authorized.");
                }

                var todoLists = await _context.List
                .Where(list => list.UserId == userId)
                .Select(list => new List
                {
                    Id = list.Id,
                    Name = list.Name,
                    Color = list.Color,
                    Tasks = _context.Task.Where(task => task.ListId == list.Id).ToList()
                })
                .ToListAsync();

                return Ok(todoLists);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<List>> CreateTodoList([FromBody] TodoListDto request)
        {
            try
            {
                var userId = ExtractUserIdFromToken(HttpContext);

                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                var user = await _context.User.FindAsync(userId);
                if (user == null)
                {
                    return BadRequest("User not found.");
                }

                var newList = new List
                {
                    Name = request.Name,
                    Color = request.Color,
                    UserId = userId,
                };

                _context.List.Add(newList);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTodoLists), new { userId = userId }, newList);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoList(long id, TodoListDto todoListDto)
        {
            try
            {
                var userId = ExtractUserIdFromToken(HttpContext);

                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                if (id <= 0 || todoListDto == null)
                {
                    return BadRequest("Invalid todo list ID or data.");
                }

                // Verificar si el usuario existe
                var user = await _context.User.FindAsync(userId);
                if (user == null)
                {
                    return Unauthorized("User not authorized.");
                }

                var existingTodoList = await _context.List.FindAsync(id);
                if (existingTodoList == null)
                {
                    return NotFound("Todo list not found.");
                }

                // Verificar si la lista pertenece al usuario
                if (existingTodoList.UserId != userId)
                {
                    return Unauthorized("User not authorized to update this todo list.");
                }

                existingTodoList.Name = todoListDto.Name;
                existingTodoList.Color = todoListDto.Color;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoList(long id)
        {
            try
            {
                var userId = ExtractUserIdFromToken(HttpContext);

                if (userId <= 0)
                {
                    return BadRequest("Invalid user ID.");
                }

                if (id <= 0)
                {
                    return BadRequest("Invalid todo list ID.");
                }

                var user = await _context.User.FindAsync(userId);
                if (user == null)
                {
                    return Unauthorized("User not authorized.");
                }

                var todoList = await _context.List.FindAsync(id);
                if (todoList == null)
                {
                    return NotFound("Todo list not found.");
                }

                if (todoList.UserId != userId)
                {
                    return Unauthorized("User not authorized to delete this todo list.");
                }

                _context.List.Remove(todoList);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred: {ex.Message}");
            }
        }

        private bool TodoListExists(long id)
        {
            return _context.List.Any(e => e.Id == id);
        }

        private long ExtractUserIdFromToken(HttpContext context)
        {
            if (context.Items.TryGetValue("UserId", out var userIdObj) && userIdObj is string userIdClaim)
            {
                if (long.TryParse(userIdClaim, out long userId))
                {
                    return userId;
                }
            }
            
            throw new InvalidOperationException("Unable to extract user ID from token.");
                }
    }
}