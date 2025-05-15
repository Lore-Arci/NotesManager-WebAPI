using System.ComponentModel.DataAnnotations;

public class Favourite {
    public int UserId { get; set; }
    public int NoteId { get; set; }
    public User User { get; set; }
    public Note Note { get; set; }
}