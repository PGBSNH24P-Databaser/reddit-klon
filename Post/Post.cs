public class Post {
    public Guid Id { get; init; }
    public User? User { get; init; }
    public Guid? ParentPostId { get; init; }
    public required string Content {get; set; }
    public DateTime CreatedDateTime { get; init; }
}