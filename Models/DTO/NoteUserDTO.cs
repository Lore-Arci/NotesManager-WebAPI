public record NoteUserDTO
{
    public int id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public bool isPublic { get; set; }
    public Category category { get; set; }
    public UserInfoDTO userInfoDTO { get; set; }
};