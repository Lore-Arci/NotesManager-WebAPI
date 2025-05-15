using System.ComponentModel.DataAnnotations;

public class Review
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Description { get; set; }
    public int? Stars { get; set; }
    public User User { get; set; }
    public Note Note { get; set; }
}