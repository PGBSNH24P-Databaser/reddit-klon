public class RegisterUserCommand : Command
{
    public RegisterUserCommand(IUserService userService, IMenuService menuService, IPostService postService) : base("register-user", "Create a new user account.", userService, menuService, postService)
    {
    }

    public override void Execute(string[] args)
    {
        string username = args[1];
        string password = args[2];

        User user = userService.RegisterUser(username, password);

        Console.WriteLine($"Created user '{user.Name}'");
    }
}