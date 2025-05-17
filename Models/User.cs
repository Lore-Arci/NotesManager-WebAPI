using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Surname { get; set; }
    [Required]
    public string Email { get; set; }
    public bool isPublic { get; set; }
    [Required]
    public string HashedPassword { get; set; }
    public byte[] Salt { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    [JsonIgnore]
    public virtual ICollection<Note> Notes { get; set; }
    [JsonIgnore]
    public virtual ICollection<Review> Reviews { get; set; }
    [JsonIgnore] 
    public virtual ICollection<Favourite> Favourites { get; set; }
}