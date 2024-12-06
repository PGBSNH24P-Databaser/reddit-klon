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

        var sql = @"INSERT INTO posts (post_id, user_id, parent_post_id, original_post_id, content, creation_timestamp) VALUES (
            @id,
            @user_id,
            NULL,
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

    public List<Post> GetAllPosts() {
        var sql = @"SELECT posts.post_id, posts.user_id, posts.parent_post_id, posts.original_post_id, posts.content, posts.creation_timestamp, users.user_id, users.name FROM posts LEFT JOIN users ON users.user_id = posts.user_id WHERE original_post_id IS NULL";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        
        using var reader = cmd.ExecuteReader();
        
        List<Post> posts = new List<Post>();
        while (reader.Read()) {
            Post post = new Post {
                Id = reader.GetGuid(0),
                User = reader.IsDBNull(1) ? null : new User {
                    Id = reader.GetGuid(1),
                    Name = reader.GetString(7),
                    Password = "" // Vi behöver inte använda lösenordet så vi skippar den
                },
                ParentPostId = reader.IsDBNull(2) ? null : reader.GetGuid(2),
                OriginalPostId = reader.IsDBNull(3) ? null : reader.GetGuid(3),
                Content = reader.GetString(4),
                CreatedDateTime = reader.GetDateTime(5)
            };

            posts.Add(post);
        }

        return posts;
    }

    public List<Post> GetAllCommentsForPost(Guid postId)
    {
        var sql = @"SELECT posts.post_id, posts.user_id, posts.parent_post_id, posts.original_post_id, posts.content, posts.creation_timestamp, users.user_id, users.name FROM posts LEFT JOIN users ON users.user_id = posts.user_id WHERE original_post_id = @id";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@id", postId);
        
        using var reader = cmd.ExecuteReader();
        
        List<Post> posts = new List<Post>();
        while (reader.Read()) {
            Post post = new Post {
                Id = reader.GetGuid(0),
                User = reader.IsDBNull(1) ? null : new User {
                    Id = reader.GetGuid(1),
                    Name = reader.GetString(7),
                    Password = "" // Vi behöver inte använda lösenordet så vi skippar den
                },
                ParentPostId = reader.IsDBNull(2) ? null : reader.GetGuid(2),
                OriginalPostId = reader.IsDBNull(3) ? null : reader.GetGuid(3),
                Content = reader.GetString(4),
                CreatedDateTime = reader.GetDateTime(5)
            };

            posts.Add(post);
        }

        return posts;
    }
}