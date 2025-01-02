using System.Security.Cryptography;
using System.Text;
using Npgsql;

// Vår PostgreSQL implementation av IUserService.
// Den "hanterar" användare genom en PostgreSQL databas.
public class PostgresUserService : IUserService
{
    // Detta objekt håller koll på kopplingen mellan databas och program kod
    // Vi använder denna när vi behöver kommunicera med databas, till exempel vid INSERTs och SELECTs.
    private NpgsqlConnection connection;
    // Denna variabel håller koll på vem som är inloggad.
    // Om värdet är null så är ingen inloggad.
    // Endast en användare kan vara inloggad vid ett tillfälle.
    // Anledningen till att detta är ett Guid är för att användar datan alltid skall vara synkad.
    private Guid? loggedInUser = null;

    // Vi behöver endast en NpgsqlConnection - som kan återanvändas - och därför tar vi in den med hjälp av en constructor.
    public PostgresUserService(NpgsqlConnection connection)
    {
        this.connection = connection;
    }

    // Denna funktion är till för att hämta information om vem som är inloggad - användar data.
    // Den måste kommunicera med databasen eftersom vi endast sparar ett Guid i "loggedInUser".
    public User? GetLoggedInUser()
    {
        // Om ingen är inlogged så kan vi returnera null eftersom det inte finns något att hämta från databasen.
        if (loggedInUser == null)
        {
            return null;
        }

        // Försök att leta upp användaren med matchande id
        var sql = @"SELECT * FROM users WHERE user_id = @id";
        using var cmd = new NpgsqlCommand(sql, this.connection);
        // Använd parameters istället för string concatenation för att undvika SQL injections
        cmd.Parameters.AddWithValue("@id", loggedInUser);

        using var reader = cmd.ExecuteReader();
        // Om ingen användare matchade - returnera null
        if (!reader.Read())
        {
            return null;
        }

        // (annars) läs informationen från queryns resultat och returnera det i form av ett User objekt

        var user = new User
        {
            Id = reader.GetGuid(0),
            Name = reader.GetString(1),
            Password = reader.GetString(2)
        };

        return user;
    }

    // Anropa denna funktion för att logga in - och skicka in användarnamn och lösenord
    // Den returnerar ett objekt med information om användaren (ifall inloggningen lyckas), eller null om inloggningen misslyckas
    public User? Login(string username, string password)
    {
        // Med username och password - försök att hitta en matchande användare.
        var sql = @"SELECT * FROM users WHERE name = @username";
        // Använd parameters istället för string concatenation för att undvika SQL injections
        using var cmd = new NpgsqlCommand(sql, this.connection);
        cmd.Parameters.AddWithValue("@username", username);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var user = new User
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1),
                Password = reader.GetString(2)
            };

            string[] passwordSplit = user.Password.Split(":");
            string passwordHash = passwordSplit[0];
            string salt = passwordSplit[1];

            byte[] fullBytes = Encoding.UTF8.GetBytes(password + salt);
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                byte[] hash = algorithm.ComputeHash(fullBytes);
                password = GetHexString(hash);
            }

            if (!passwordHash.Equals(password))
            {
                continue;
            }

            // hämta information från resultat och returnera det i form av ett User objekt
            loggedInUser = user.Id;
            return user;
        }

        // Om ingen användare matchade - returner null
        return null;
    }

    public void Logout()
    {
        loggedInUser = null;
    }

    public User RegisterUser(string username, string password)
    {
        // Generate salt
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        string saltString = GetHexString(salt);

        byte[] fullBytes = Encoding.UTF8.GetBytes(password + saltString);

        using (HashAlgorithm algorithm = SHA256.Create())
        {
            byte[] hash = algorithm.ComputeHash(fullBytes);
            password = GetHexString(hash);
        }

        password += ":" + saltString;

        var user = new User
        {
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

    private static string GetHexString(byte[] array)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in array)
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }
}