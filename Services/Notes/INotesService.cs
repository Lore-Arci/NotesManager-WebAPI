using Microsoft.AspNetCore.Mvc;

public interface INotesService
{
    Task<List<Note>> GetPublicNotesAsync();
    Task<Note> PostNoteAsync(NoteDTO noteDTO);
    Task<List<NoteUserDTO>> GetNotesByCategoryAsync(Category? category);
    Task<List<NoteInfoDTO>> GetNotesByUserId(int userId);   
    Task<Note> UpdateNoteVisibility(int id);
}