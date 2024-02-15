using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using To_Do_List_Back.Models;

namespace To_Do_List_Back.Controllers
{
    public class CreateTaskRequest
    {
        public long ListId { get; set; }
        public TodoTask Task { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ListContext _context;

        public TaskController(ListContext context)
        {
            _context = context;
        }

        [HttpGet("{listId}")]
        public async Task<ActionResult<IEnumerable<TodoTask>>> GetTasks(long listId)
        {
            if (listId <= 0)
            {
                return BadRequest("Invalid list ID.");
            }

            var todoList = await _context.List.FindAsync(listId);
            if (todoList == null)
            {
                return NotFound("Todo list not found.");
            }

            var tasks = await _context.Task
                                        .Where(task => task.TodoListId == listId)
                                        .ToListAsync();

            return Ok(tasks);
        }

        [HttpPost]
        public async Task<ActionResult<TodoTask>> CreateTask([FromBody] CreateTaskRequest request)
        {
            if (request == null || request.ListId <= 0 || request.Task == null)
            {
                return BadRequest("Invalid list ID or task.");
            }

            var todoList = await _context.List.FindAsync(request.ListId);
            if (todoList == null)
            {
                return NotFound("Todo list not found.");
            }

            var newTask = new TodoTask
            {
                Title = request.Task.Title,
                Completed = false,
                TodoListId = request.ListId
            };

            _context.Task.Add(newTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasks), new { listId = request.ListId }, newTask);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(long id, TodoTask task)
        {
            if (id <= 0 || task == null)
            {
                return BadRequest("Invalid task ID or data.");
            }

            var existingTask = await _context.Task.FindAsync(id);
            if (existingTask == null)
            {
                return NotFound("Task not found.");
            }

            existingTask.Title = task.Title;
            existingTask.Completed = task.Completed;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskExists((int)id))
                {
                    return NotFound("Task not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid task ID.");
            }

            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            _context.Task.Remove(task);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TaskExists(int id)
        {
            return _context.Task.Any(e => e.Id == id);
        }
    }
}
