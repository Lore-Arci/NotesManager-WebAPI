using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Category
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Description { get; set; }
    [JsonIgnore]
    public virtual ICollection<Note> Notes { get; set; }
}