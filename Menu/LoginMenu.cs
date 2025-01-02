public class LoginMenu : Menu
{
    public LoginMenu(IUserService userService, IMenuService menuService, IPostService postService)
    {
        AddCommand(new LoginCommand(userService, menuService, postService));
        AddCommand(new RegisterUserCommand(userService, menuService, postService));
    }

    public override void Display()
    {
        Console.ReadKey(intercept: true);
        Console.WriteLine("Welcome! Please login by typing 'login'");
    }
}