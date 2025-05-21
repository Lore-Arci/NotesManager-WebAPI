public record NoteInfoDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public bool isPublic { get; set; }
    public Category Category { get; set; }
}