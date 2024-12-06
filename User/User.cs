// En databas model som representerar en användare (en rad i users tabellen).
public class User {
    public Guid Id { get; init; }
    public string Name { get; set; }
    public string Password { get; set; }
}