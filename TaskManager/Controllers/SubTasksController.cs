using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class SubTasksController : ControllerBase
{
    private readonly AppDbContext _context;

    public SubTasksController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<SubTask>> PostSubTask(SubTask subTask)
    {
        _context.SubTasks.Add(subTask);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetSubTask), new { id = subTask.Id }, subTask);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubTask>> GetSubTask(int id)
    {
        var subTask = await _context.SubTasks.FindAsync(id);

        if (subTask == null)
        {
            return NotFound();
        }

        return Ok(subTask);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutSubTask(int id, SubTask updatedSubTask)
    {
        var subTask = await _context.SubTasks.FindAsync(id);

        if (subTask == null)
        {
            return NotFound();
        }

        subTask.Name = updatedSubTask.Name;
        subTask.Description = updatedSubTask.Description;
        subTask.IsComplete = updatedSubTask.IsComplete;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubTask(int id)
    {
        var subTask = await _context.SubTasks.FindAsync(id);

        if (subTask == null)
        {
            return NotFound();
        }

        _context.SubTasks.Remove(subTask);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
