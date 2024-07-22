using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public TasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<Task>> PostTask(Task task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Task>> GetTask(int id)
    {
        var task = await _context.Tasks.Include(t => t.SubTasks).FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        var response = new
        {
            task.Id,
            task.Name,
            task.Description,
            task.IsComplete,
            SubTaskCount = task.SubTasks.Count
        };
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTask(int id, Task updatedTask)
    {
        var task = await _context.Tasks.Include(t => t.SubTasks).FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        if (task.SubTasks.Any(st => !st.IsComplete))
        {
            return BadRequest("Task cannot be marked complete until all sub-tasks are complete.");
        }

        task.Name = updatedTask.Name;
        task.Description = updatedTask.Description;
        task.IsComplete = updatedTask.IsComplete;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var task = await _context.Tasks.Include(t => t.SubTasks).FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
        {
            return NotFound();
        }

        if (task.SubTasks.Any())
        {
            return BadRequest("Cannot delete a task with existing sub-tasks.");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}