namespace reddit_clone;

using Npgsql;

/*

users:
- user_id (PRIMARY KEY)
- name
- password

posts: (inlägg eller kommentar)
- post_id (PRIMARY KEY)
- user_id (FOREIGN KEY -> users)
- parent_post_id NULLABLE (FOREIGN KEY -> posts)
- content
- creation timestamp

user_likes:
- user_id (FOREIGN KEY -> users)
- post_id (FOREIGN KEY -> posts)

*/

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Host=localhost;Username=postgres;Password=password;Database=reddit";
        using var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        
        var createTablesSql = @"
            CREATE TABLE IF NOT EXISTS users (
                user_id UUID PRIMARY KEY,
                name TEXT,
                password TEXT
            );

            CREATE TABLE IF NOT EXISTS posts (
                post_id UUID PRIMARY KEY,
                user_id UUID REFERENCES users(user_id),
                parent_post_id UUID REFERENCES posts(post_id),
                content TEXT,
                creation_timestamp TIMESTAMP
            );

            CREATE TABLE IF NOT EXISTS user_likes (
                user_id UUID REFERENCES users(user_id),
                post_id UUID REFERENCES posts(post_id)
            );
        ";

        using var createTableCmd = new NpgsqlCommand(createTablesSql, connection);
        createTableCmd.ExecuteNonQuery();

        var userService = new PostgresUserService(connection);

        /*userService.RegisterUser("Ironman", "tonystark");
        userService.RegisterUser("Superman", "superstrong");
        userService.RegisterUser("Batman", "awesome");*/

        User? user = userService.Login("Ironman", "tonystark");
        if (user != null) {
            Console.WriteLine(user.Id);
        } else {
            Console.WriteLine("Wrong username or password");
        }
    }
}