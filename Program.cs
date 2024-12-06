namespace reddit_clone;

using Npgsql;

/*

users:
- user_id (PRIMARY KEY)
- name
- password

posts: (inlägg eller kommentar)
- post_id (PRIMARY KEY)
- user_id (FOREIGN KEY -> users) (ON DELETE SET NULL)
- parent_post_id NULLABLE (FOREIGN KEY -> posts)
- content
- creation timestamp

user_likes:
- user_id (FOREIGN KEY -> users) (ON DELETE CASCADE)
- post_id (FOREIGN KEY -> posts) (ON DELETE CASCADE)

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
                user_id UUID REFERENCES users(user_id) ON DELETE SET NULL,
                parent_post_id UUID REFERENCES posts(post_id), -- Kopplar till inlägget/kommentaren ovanför i hierarkiet
                original_post_id UUID REFERENCES posts(post_id), -- Kopplar till det originella inlägget (vi har lagt in detta i efterhand) 
                content TEXT,
                creation_timestamp TIMESTAMP
            );

            CREATE TABLE IF NOT EXISTS user_likes (
                user_id UUID REFERENCES users(user_id) ON DELETE CASCADE,
                post_id UUID REFERENCES posts(post_id) ON DELETE CASCADE
            );
        ";

        using var createTableCmd = new NpgsqlCommand(createTablesSql, connection);
        createTableCmd.ExecuteNonQuery();

        IUserService userService = new PostgresUserService(connection);
        IPostService postService = new PostgresPostService(userService, connection);
        IMenuService menuService = new SimpleMenuService();
        Menu initialMenu = new LoginMenu(userService, menuService, postService);
        menuService.SetMenu(initialMenu);

        while(true) {
            string? inputCommand = Console.ReadLine();
            if (inputCommand != null) {
                menuService.GetMenu().ExecuteCommand(inputCommand);
            } else {
                break;
            }
        }

        // Denna kod ligger bara som referens
        /*var userService = new PostgresUserService(connection);
        userService.RegisterUser("Ironman", "tonystark");
        userService.RegisterUser("Superman", "superstrong");
        userService.RegisterUser("Batman", "awesome");*/

        /*User? user = userService.Login("Ironman", "tonystark");
        if (user != null) {
            Console.WriteLine(user.Id);
        } else {
            Console.WriteLine("Wrong username or password");
        }*/
    }
}