using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using To_Do_List_Back.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace To_Do_List_Back.Controllers
{
    public class TodoListRequest
    {
        public long UserID { get; set; }
        public TodoListDto TodoList { get; set; }
    }

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

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<List>>> GetTodoLists(long userId)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID.");
            }

            var user = await _context.User.FindAsync(userId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var todoLists = await _context.List
                                        .Where(list => list.UserId == userId)
                                        .ToListAsync();

            if (todoLists == null || !todoLists.Any())
            {
                return NotFound("Lists not found for the specified user ID.");
            }

            return Ok(todoLists);
        }

        [HttpPost]
        public async Task<ActionResult<List>> CreateTodoList([FromBody] TodoListRequest request)
        {
            if (request == null || request.UserID <= 0 || request.TodoList == null)
            {
                return BadRequest("Invalid user ID or todo list.");
            }

            var user = await _context.User.FindAsync(request.UserID);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var newList = new List
            {
                Name = request.TodoList.Name,
                Color = request.TodoList.Color,
                UserId = request.UserID,
            };

            _context.List.Add(newList);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoLists), new { userId = request.UserID }, newList);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodoList(long id, TodoListDto todoListDto)
        {
            if (id <= 0 || todoListDto == null)
            {
                return BadRequest("Invalid todo list ID or data.");
            }

            var existingTodoList = await _context.List.FindAsync(id);
            if (existingTodoList == null)
            {
                return NotFound("Todo list not found.");
            }

            existingTodoList.Name = todoListDto.Name;
            existingTodoList.Color = todoListDto.Color;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoListExists((int)id))
                {
                    return NotFound("Todo list not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoList(long id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid todo list ID.");
            }

            var todoList = await _context.List.FindAsync(id);
            if (todoList == null)
            {
                return NotFound("Todo list not found.");
            }

            _context.List.Remove(todoList);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoListExists(int id)
        {
            return _context.List.Any(e => e.Id == id);
        }
    }
}