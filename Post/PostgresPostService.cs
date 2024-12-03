using Npgsql;

public class PostgresPostService : IPostService {
    private IUserService userService;
    private NpgsqlConnection connection;

    public PostgresPostService(IUserService userService, NpgsqlConnection connection) {
        this.userService = userService;
        this.connection = connection;
    }

    public Post CreatePost(string content)
    {
        var user = userService.GetLoggedInUser();
        if (user == null) {
            throw new ArgumentException("You are not logged in.");
        }

        var post = new Post {
            Id = Guid.NewGuid(),
            User = user,
            Content = content,
            CreatedDateTime = DateTime.Now,
            ParentPostId = null,
        };

        var sql = @"INSERT INTO posts (post_id, user_id, parent_post_id, content, creation_timestamp) VALUES (
            @id,
            @user_id,
            NULL,
            @content,
            @created_date
        )";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@id", post.Id);
        cmd.Parameters.AddWithValue("@user_id", post.User.Id);
        cmd.Parameters.AddWithValue("@content", post.Content);
        cmd.Parameters.AddWithValue("@created_date", post.CreatedDateTime);
        
        cmd.ExecuteNonQuery();

        return post;
    }
}