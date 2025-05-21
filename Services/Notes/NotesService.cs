using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

public class NotesService(NotesContext db) : INotesService
{
    public async Task<List<Note>> GetPublicNotesAsync()
    {
        var publicNotes = await db.Notes
            .Where(note => note.IsPublic)
            .Include(note => note.User)
            .ToListAsync();

        if (publicNotes == null) return null;

        return publicNotes;
    }

    public async Task<Note> PostNoteAsync(NoteDTO noteDTO)
    {
        if (noteDTO is null) return null;

        var user = await db.Users.FirstOrDefaultAsync(u => u.Id == noteDTO.userId);

        if (user is null) return null;

        var note = new Note
        {
            Title = noteDTO.title,
            Description = noteDTO.description,
            IsPublic = noteDTO.isPublic,
            Category = noteDTO.category,
            User = user
        };

        await db.Notes.AddAsync(note);
        await db.SaveChangesAsync();

        return note;
    }

    public async Task<List<NoteUserDTO>> GetNotesByCategoryAsync(Category? category)
    {
        if (category is null) return null;
        var notes = await db.Notes
            .Where(note => note.Category == category)
            .Include(note => note.User)
            .Select(note => new NoteUserDTO
            {
                id = note.Id,
                title = note.Title,
                description = note.Description,
                isPublic = note.IsPublic,
                category = note.Category,
                userInfoDTO = new UserInfoDTO
                {
                    Name = note.User.Name,
                    Email = note.User.Email
                }
            })
            .ToListAsync();

        if (notes == null) return null;
        return notes;
    }

    public async Task<List<NoteInfoDTO>> GetNotesByUserId(int userId)
    {
        var user = await db.Users.FindAsync(userId);

        if (user is null) return null;

        var notes = await db.Notes.
            Where(note => note.User.Id == userId)
            .Select(note => new NoteInfoDTO
            {
                Title = note.Title,
                Description = note.Description,
                isPublic = note.IsPublic,
                Category = note.Category,
            })
            .ToListAsync();

        if (notes == null) return null;

        return notes;
    }

    public async Task<Note> UpdateNoteVisibility(int id)
    {
        var note = await db.Notes.FindAsync(id);

        if (note is null) return null;

        note.IsPublic = !note.IsPublic;

        db.Notes.Update(note);
        await db.SaveChangesAsync();

        return note;
    }
}