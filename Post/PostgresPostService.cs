using System.Data.Common;
using Npgsql;

// Vår PostgreSQL implementation av IPostService.
// Den "hanterar" inlägg genom en PostgreSQL databas.
public class PostgresPostService : IPostService
{
    // För att veta vem som är inloggad (vilket exempelvis behövs när man skapar inlägg) hämtas IUserService in
    private IUserService userService;
    // Detta objekt håller koll på kopplingen mellan databas och program kod
    // Vi använder denna när vi behöver kommunicera med databas, till exempel vid INSERTs och SELECTs.
    private NpgsqlConnection connection;

    public PostgresPostService(IUserService userService, NpgsqlConnection connection)
    {
        this.userService = userService;
        this.connection = connection;
    }

    // Ansvarar för att skapa ett nytt huvudinlägg (men inte kommentarer).
    public Post CreatePost(string content)
    {
        // Hämta inlogad användare, eller kasta exception om ingen är inloggad.
        var user = userService.GetLoggedInUser();
        if (user == null)
        {
            throw new ArgumentException("You are not logged in.");
        }

        // Skapa ett objekt som representerar inlägget
        var post = new Post
        {
            Id = Guid.NewGuid(),
            User = user,
            Content = content,
            CreatedDateTime = DateTime.Now,
            ParentPostId = null,
            OriginalPostId = null,
        };

        // Kör SQL kod för att spara inlägget i databasen
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

    // Denna hämtar alla inlägg från databasen, men bara huvudinlägg och inte kommentarer
    public List<Post> GetAllPosts()
    {
        // SQL kod för att hämta alla inlägg.
        var sql = @"SELECT posts.post_id, posts.user_id, posts.parent_post_id, posts.original_post_id, posts.content, posts.creation_timestamp, users.user_id, users.name FROM posts LEFT JOIN users ON users.user_id = posts.user_id WHERE original_post_id IS NULL";
        using var cmd = new NpgsqlCommand(sql, this.connection);

        using var reader = cmd.ExecuteReader();

        // Loopa igenom alla rader ifrån resultatet och lägg in dem, i form av objekt, i listan nedanför.
        List<Post> posts = new List<Post>();
        while (reader.Read())
        {
            // Hämta ut all information från raden och lägg in den i ett Post objekt
            Post post = new Post
            {
                Id = reader.GetGuid(0),
                // Vi har joinat med users tabellen och kan därför också hämta ut information om användaren
                User = reader.IsDBNull(1) ? null : new User
                {
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

    // Hämtar alla kommentarer för ett visst inlägg
    public List<Post> GetAllCommentsForPost(Guid postId)
    {
        var sql = @"SELECT posts.post_id, posts.user_id, posts.parent_post_id, posts.original_post_id, posts.content, posts.creation_timestamp, users.user_id, users.name FROM posts LEFT JOIN users ON users.user_id = posts.user_id WHERE original_post_id = @id";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@id", postId);

        using var reader = cmd.ExecuteReader();

        // Upprepar koden från funktionen ovanför (vi borde bryta ut koden så att den inte upprepas).
        List<Post> posts = new List<Post>();
        while (reader.Read())
        {
            Post post = new Post
            {
                Id = reader.GetGuid(0),
                User = reader.IsDBNull(1) ? null : new User
                {
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

    // Denna funktion används för att skapa kommentarer och inte "vanliga"/huvud-inlägg
    public void AddCommentToPost(string content, Guid parentPostId, Guid originalPostId)
    {
        // Hämta inloggad användare, eller kasta exception om ingen är inloggad.
        var user = userService.GetLoggedInUser();
        if (user == null)
        {
            throw new ArgumentException("You are not logged in.");
        }

        // Skapa ett objekt som representerar inlägget
        var post = new Post
        {
            Id = Guid.NewGuid(),
            User = user,
            Content = content,
            CreatedDateTime = DateTime.Now,
            ParentPostId = parentPostId,
            OriginalPostId = originalPostId,
        };

        // Kör SQL kod för att spara inlägget i databasen
        var sql = @"INSERT INTO posts (post_id, user_id, parent_post_id, original_post_id, content, creation_timestamp) VALUES (
            @id,
            @user_id,
            @parent_post_id,
            @original_post_id,
            @content,
            @created_date
        )";

        // Det är extremt viktigt att vi INTE använder string concatenation. Annars utsätter vi oss för SQL injections.
        // string sql = "INSERT INTO ..." + post.Id + content;

        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@id", post.Id);
        cmd.Parameters.AddWithValue("@user_id", post.User.Id);
        cmd.Parameters.AddWithValue("@parent_post_id", post.ParentPostId);
        cmd.Parameters.AddWithValue("@original_post_id", post.OriginalPostId);
        cmd.Parameters.AddWithValue("@content", post.Content);
        cmd.Parameters.AddWithValue("@created_date", post.CreatedDateTime);

        cmd.ExecuteNonQuery();
    }
}