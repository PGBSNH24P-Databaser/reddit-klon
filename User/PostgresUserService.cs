using Npgsql;

public class PostgresUserService : IUserService
{
    private NpgsqlConnection connection;
    private Guid? loggedInUser = null;

    public PostgresUserService(NpgsqlConnection connection) {
        this.connection = connection;
    }

    public User? GetLoggedInUser()
    {
        if (loggedInUser == null) 
        {
            return null;
        }

        var sql = @"SELECT * FROM users WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@id", loggedInUser);
        
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) {
            return null;
        }

        var user = new User {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Password = reader.GetString(2)
        };

        return user;
    }

    public User? Login(string username, string password)
    {
        var sql = @"SELECT * FROM users WHERE name = @username AND password = @password";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);
        
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) {
            return null;
        }

        var user = new User {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Password = reader.GetString(2)
        };

        loggedInUser = user.Id;

        return user;
    }

    public void Logout()
    {
        loggedInUser = null;
    }

    public User RegisterUser(string username, string password)
    {
        var user = new User {
            Id = Guid.NewGuid(),
            Name = username,
            Password = password
        };

        var sql = @"INSERT INTO users (user_id, name, password) VALUES (
            @id,
            @name,
            @password
        )";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@id", user.Id);
        cmd.Parameters.AddWithValue("@name", user.Name);
        cmd.Parameters.AddWithValue("@password", user.Password);
        
        cmd.ExecuteNonQuery();

        return user;
    }
}