using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/[controller]")]
public class NotesController(INotesService service) : ControllerBase
{
    [HttpGet("public-notes")]
    public async Task<IActionResult> GetPublicNotes()
    {
        var publicNotes = await service.GetPublicNotesAsync();

        if (publicNotes == null) return NotFound();

        return Ok(publicNotes);
    }

    [Authorize]
    [HttpPost("post-note")]
    public async Task<IActionResult> PostNote([FromBody] NoteDTO noteDTO)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var note = await service.PostNoteAsync(noteDTO);

        if (note is null) return BadRequest("Note not created");

        return Created("Note created successfully", note);
    }

    [Authorize]
    [HttpGet("notes-by-category")]
    public async Task<IActionResult> GetNotesByCategory([FromQuery] Category category)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var notes = await service.GetNotesByCategoryAsync(category);

        if (notes == null) return NotFound();

        return Ok(notes);
    }

    [Authorize]
    [HttpGet("notes-by-userId/{id:int}")]
    public async Task<IActionResult> GetNotesByUserId(int id)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var notes = await service.GetNotesByUserId(id);

        if (notes == null) return NotFound();

        return Ok(notes);
    }

    [Authorize]
    [HttpPut("update-note-visibility/{id:int}")]
    public async Task<IActionResult> UpdateNoteVisibility(int id)
    {
        var note = await service.UpdateNoteVisibility(id);
        if (note is null) return NotFound();
        return Ok(note); 
    }
}