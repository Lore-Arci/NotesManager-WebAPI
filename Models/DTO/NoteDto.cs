using System.Security.Cryptography.X509Certificates;

public record NoteDTO(string title, string description, bool isPublic, Category category, int userId); 