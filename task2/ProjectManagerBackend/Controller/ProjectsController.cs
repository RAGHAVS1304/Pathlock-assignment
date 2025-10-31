using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagerBackend.Data;
using ProjectManagerBackend.Models;
using System.Security.Claims;

namespace ProjectManagerBackend.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ProjectsController(AppDbContext db) => _db = db;

    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var uid = GetUserId();
        var projects = await _db.Projects.Where(p => p.OwnerId == uid).ToListAsync();
        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(ProjectCreateDto dto)
    {
        var uid = GetUserId();
        var project = new Project { Title = dto.Title, Description = dto.Description, OwnerId = uid };
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
        return Ok(project);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProject(int id)
    {
        var uid = GetUserId();
        var proj = await _db.Projects.Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);

        if (proj == null) return NotFound();
        return Ok(proj);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var uid = GetUserId();
        var proj = await _db.Projects.FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == uid);
        if (proj == null) return NotFound();

        _db.Projects.Remove(proj);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}

public record ProjectCreateDto(string Title, string? Description);
