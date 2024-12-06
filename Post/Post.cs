// En databas model som representerar ett inlägg (en rad i posts tabellen).
public class Post {
    public Guid Id { get; init; }
    public User? User { get; init; }
    public Guid? ParentPostId { get; init; }
    public Guid? OriginalPostId { get; init; }
    public required string Content {get; set; }
    public DateTime CreatedDateTime { get; init; }
}