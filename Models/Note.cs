using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Note
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    public bool IsPublic { get; set; }
    public Category Category { get; set; }
    [JsonIgnore]
    public virtual ICollection<Review> Reviews { get; set; }
    [JsonIgnore]
    public virtual ICollection<Favourite> Favourites { get; set; }
    public User User { get; set; }
}