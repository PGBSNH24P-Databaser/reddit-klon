public class LoginCommand : Command
{
    public LoginCommand(IUserService userService, IMenuService menuService) : base("login", "Login with username and password.", userService, menuService)
    {
    }

    public override void Execute(string[] args)
    {
        // login [username password]
        string username = args[1];
        string password = args[2];

        User? user = userService.Login(username, password);
        if (user == null)
        {
            Console.WriteLine("Wrong username or password.");
            return;
        }

        Console.WriteLine("You successfully logged in.");
        menuService.SetMenu(new UserMenu());
    }
}